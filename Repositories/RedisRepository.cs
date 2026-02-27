using System.Text.Json;
using EvacApiProblem.Models;
using StackExchange.Redis;

namespace EvacApiProblem.Repositories
{
    /// <summary>
    /// Redis Repository
    /// </summary>
    public class RedisRepository(IConnectionMultiplexer redis) : IRedisRepository
    {
        private readonly IDatabase _db = redis.GetDatabase();

        // Redis keys
        private const string ZONES_KEY = "zones";
        private const string VEHICLES_KEY = "vehicles";
        private const string STATUS_KEY = "status";

        /// <summary>
        /// Saves evacuation zone to Redis
        /// </summary>
        public async Task SaveZoneAsync(EvacuationZone zone)
        {
            await _db.HashSetAsync(ZONES_KEY, zone.ZoneID, JsonSerializer.Serialize(zone));
        }

        /// <summary>
        /// Gets evacuation zone by ID from Redis
        /// </summary>
        public async Task<EvacuationZone?> GetZoneByIdAsync(string zoneId)
        {
            var value = await _db.HashGetAsync(ZONES_KEY, zoneId);
            if (!value.HasValue)
            {
                return null;
            }
            return JsonSerializer.Deserialize<EvacuationZone>(value.ToString());
        }

        /// <summary>
        /// Gets all evacuation zones from Redis
        /// </summary>
        public async Task<List<EvacuationZone>> GetAllZonesAsync()
        {
            var entries = await _db.HashGetAllAsync(ZONES_KEY);
            return entries
                .Select(e => JsonSerializer.Deserialize<EvacuationZone>(e.Value.ToString()))
                .OfType<EvacuationZone>()
                .ToList();
        }

        /// <summary>
        /// Saves vehicle to Redis
        /// </summary>
        public async Task SaveVehicleAsync(Vehicle vehicle)
        {
            await _db.HashSetAsync(VEHICLES_KEY, vehicle.VehicleID, JsonSerializer.Serialize(vehicle));
        }

        /// <summary>
        /// Gets vehicle by ID from Redis
        /// </summary>
        public async Task<Vehicle?> GetVehicleByIdAsync(string vehicleId)
        {
            var value = await _db.HashGetAsync(VEHICLES_KEY, vehicleId);
            if (!value.HasValue)
            {
                return null;
            }
            return JsonSerializer.Deserialize<Vehicle>(value.ToString());
        }

        /// <summary>
        /// Gets all vehicles from Redis
        /// </summary>
        public async Task<List<Vehicle>> GetAllVehiclesAsync()
        {
            var entries = await _db.HashGetAllAsync(VEHICLES_KEY);
            return entries
                .Select(e => JsonSerializer.Deserialize<Vehicle>(e.Value.ToString()))
                .OfType<Vehicle>()
                .ToList();
        }

        /// <summary>
        /// Saves evacuation status to Redis
        /// </summary>
        public async Task SaveStatusAsync(EvacuationStatus status)
        {
            await _db.HashSetAsync(STATUS_KEY, status.ZoneID, JsonSerializer.Serialize(status));
        }

        /// <summary>
        /// Gets evacuation status by zone ID from Redis
        /// </summary>
        public async Task<EvacuationStatus?> GetStatusAsync(string zoneId)
        {
            var value = await _db.HashGetAsync(STATUS_KEY, zoneId);
            if (!value.HasValue)
            {
                return null;
            }
            return JsonSerializer.Deserialize<EvacuationStatus>(value.ToString());
        }

        /// <summary>
        /// Gets all evacuation statuses from Redis
        /// </summary>
        public async Task<List<EvacuationStatus>> GetAllStatusesAsync()
        {
            var entries = await _db.HashGetAllAsync(STATUS_KEY);
            return entries
                .Select(e => JsonSerializer.Deserialize<EvacuationStatus>(e.Value.ToString()))
                .OfType<EvacuationStatus>()
                .ToList();
        }

        /// <summary>
        /// Clears all data from Redis
        /// </summary>
        public async Task ClearAllDataAsync()
        {
            await _db.KeyDeleteAsync([ZONES_KEY, VEHICLES_KEY, STATUS_KEY]);
        }

        /// <summary>
        /// Acquires lock in Redis
        /// </summary>
        public async Task<bool> AcquireLockAsync(string lockKey, TimeSpan expiration)
        {
            return await _db.StringSetAsync(lockKey, "locked", expiration, When.NotExists);
        }

        /// <summary>
        /// Releases lock in Redis
        /// </summary>
        public async Task ReleaseLockAsync(string lockKey)
        {
            await _db.KeyDeleteAsync(lockKey);
        }
    }
}