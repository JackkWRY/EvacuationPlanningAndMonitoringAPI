using EvacApiProblem.Helpers;
using EvacApiProblem.Models;
using EvacApiProblem.Repositories;

namespace EvacApiProblem.Services
{
    /// <summary>
    /// Evacuation service implementation
    /// </summary>
    public class EvacuationService(IRedisRepository redisRepository, ILogger<EvacuationService> logger) : IEvacuationService
    {
        /// <summary>
        /// Add evacuation zone to Redis
        /// </summary>
        public async Task AddEvacuationZoneAsync(EvacuationZone zone)
        {
            await redisRepository.SaveZoneAsync(zone);
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation(
                    "Zone added: {ZoneID} at ({Lat}, {Lon}), {People} people, urgency {Urgency}.",
                    zone.ZoneID, zone.LocationCoordinates.Latitude, zone.LocationCoordinates.Longitude,
                    zone.NumberOfPeople, zone.UrgencyLevel
                );
            }
        }

        /// <summary>
        /// Generate evacuation plan based on zones, vehicles, and statuses
        /// </summary>
        public async Task<List<EvacuationPlan>> GeneratePlanAsync()
        {
            var allZones = await redisRepository.GetAllZonesAsync();
            var allVehicles = await redisRepository.GetAllVehiclesAsync();
            var allStatuses = await redisRepository.GetAllStatusesAsync();

            if (allZones.Count == 0)
            {
                throw new InvalidOperationException("No evacuation zones added.");
            }

            if (allVehicles.Count == 0)
            {
                throw new InvalidOperationException("No vehicles available.");
            }

            var plans = new List<EvacuationPlan>();
            var availableVehicles = allVehicles.ToList();

            // Sort zones by urgency level
            var prioritizedZones = allZones.OrderByDescending(z => z.UrgencyLevel).ToList();

            foreach (var zone in prioritizedZones)
            {
                // Calculate remaining people and skip if the zone is already fully evacuated.
                var status = allStatuses.FirstOrDefault(s => s.ZoneID == zone.ZoneID);
                int remainingPeople = status != null ? status.RemainingPeople : zone.NumberOfPeople;
                if (remainingPeople <= 0)
                {
                    continue;
                }

                while (remainingPeople > 0 && availableVehicles.Count > 0)
                {
                    // Calculate distance for all available vehicles to the current zone
                    var vehicleDistances = availableVehicles.Select(v => new
                    {
                        Vehicle = v,
                        Distance = HaversineHelper.CalculateDistance(v.LocationCoordinates, zone.LocationCoordinates)
                    }).ToList();

                    // Find best fit vehicle
                    // Filter by capacity, then sort by smallest capacity and shortest distance
                    var bestFit = vehicleDistances
                        .Where(vd => vd.Vehicle.Capacity >= remainingPeople)
                        .OrderBy(vd => vd.Vehicle.Capacity)
                        .ThenBy(vd => vd.Distance)
                        .FirstOrDefault();

                    // Fallback to largest vehicle if the best fit vehicle is not found
                    // Exit allocation process if available vehicles list is exhausted
                    var bestMatch = bestFit ?? vehicleDistances
                        .OrderByDescending(vd => vd.Vehicle.Capacity)
                        .ThenBy(vd => vd.Distance)
                        .FirstOrDefault();
                    if (bestMatch == null)
                    {
                        break;
                    }

                    var vehicle = bestMatch.Vehicle;

                    // Acquire Redis to lock vehicle to prevent concurrent assignment
                    string lockKey = $"lock:vehicle:{vehicle.VehicleID}";
                    bool isLocked = await redisRepository.AcquireLockAsync(lockKey, TimeSpan.FromSeconds(30));
                    if (!isLocked)
                    {
                        if (logger.IsEnabled(LogLevel.Warning))
                        {
                            logger.LogWarning("Vehicle {VehicleID} locked, skipping.", vehicle.VehicleID);
                        }
                        availableVehicles.Remove(vehicle);
                        continue;
                    }

                    try
                    {
                        int peopleToEvacuate = Math.Min(remainingPeople, vehicle.Capacity);

                        // Calculate travel time in hours, then convert to rounded-up minutes for ETA
                        double timeHours = bestMatch.Distance / vehicle.Speed;
                        int etaMinutes = (int)Math.Ceiling(timeHours * 60);

                        var plan = new EvacuationPlan
                        {
                            ZoneID = zone.ZoneID,
                            VehicleID = vehicle.VehicleID,
                            ETA = etaMinutes,
                            NumberOfPeople = peopleToEvacuate
                        };
                        plans.Add(plan);

                        if (logger.IsEnabled(LogLevel.Information))
                        {
                            logger.LogInformation(
                                "Plan: {VehicleID} -> Zone {ZoneID}, {Distance:F2} km, ETA {ETA} min, {People} people.",
                                vehicle.VehicleID, zone.ZoneID, bestMatch.Distance, etaMinutes, peopleToEvacuate
                            );
                        }

                        remainingPeople -= peopleToEvacuate;
                        availableVehicles.Remove(vehicle);
                    }
                    finally
                    {
                        await redisRepository.ReleaseLockAsync(lockKey);
                    }
                }

                if (remainingPeople > 0 && logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning(
                        "Zone {ZoneID}: {Remaining} people still need evacuation (urgency {Urgency}).",
                        zone.ZoneID, remainingPeople, zone.UrgencyLevel
                    );
                }
            }

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation(
                    "Plan complete: {PlanCount} assignments for {ZoneCount} zones.",
                    plans.Count, prioritizedZones.Count
                );
            }

            return plans;
        }

        /// <summary>
        /// Get all evacuation statuses
        /// </summary>
        public async Task<List<EvacuationStatus>> GetAllStatusesAsync()
        {
            var allZones = await redisRepository.GetAllZonesAsync();
            var allStatuses = await redisRepository.GetAllStatusesAsync();

            // Map status to each zone
            // If zone has no status yet, create a default status
            return allZones.Select(zone =>
            {
                var status = allStatuses.FirstOrDefault(s => s.ZoneID == zone.ZoneID);
                return status ?? new EvacuationStatus
                {
                    ZoneID = zone.ZoneID,
                    TotalEvacuated = 0,
                    RemainingPeople = zone.NumberOfPeople,
                    LastVehicleUsed = null
                };
            }).ToList();
        }

        /// <summary>
        /// Update evacuation status
        /// </summary>
        public async Task UpdateStatusAsync(string zoneId, string vehicleId, int evacuatedCount)
        {
            // Validate specified zone and vehicle exist
            var zone = await redisRepository.GetZoneByIdAsync(zoneId)
                ?? throw new KeyNotFoundException($"Zone '{zoneId}' not found.");
            var vehicle = await redisRepository.GetVehicleByIdAsync(vehicleId)
                ?? throw new KeyNotFoundException($"Vehicle '{vehicleId}' not found.");

            // Get current status or initialize new one
            var status = await redisRepository.GetStatusAsync(zoneId);
            status ??= new EvacuationStatus
            {
                ZoneID = zoneId,
                TotalEvacuated = 0,
                RemainingPeople = zone.NumberOfPeople,
                LastVehicleUsed = null
            };

            if (evacuatedCount > status.RemainingPeople)
            {
                throw new InvalidOperationException(
                    $"Cannot evacuate {evacuatedCount} from Zone '{zoneId}'. Only {status.RemainingPeople} remaining."
                );
            }

            // Update evacuation progress
            status.TotalEvacuated += evacuatedCount;
            status.RemainingPeople -= evacuatedCount;
            status.LastVehicleUsed = vehicleId;

            await redisRepository.SaveStatusAsync(status);
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation(
                    "Status updated: Zone {ZoneID}, evacuated {Count} via {VehicleID}. Total: {Total}, remaining: {Remaining}.",
                    zoneId, evacuatedCount, vehicleId, status.TotalEvacuated, status.RemainingPeople
                );
            }
        }

        /// <summary>
        /// Clear all evacuation data
        /// </summary>
        public async Task ClearAllDataAsync()
        {
            await redisRepository.ClearAllDataAsync();
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("All evacuation data cleared.");
            }
        }
    }
}