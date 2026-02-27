using EvacApiProblem.Models;
using EvacApiProblem.Repositories;

namespace EvacApiProblem.Services
{
    public class VehicleService(IRedisRepository redisRepository, ILogger<VehicleService> logger) : IVehicleService
    {
        /// <summary>
        /// Add vehicle to Redis
        /// </summary>
        public async Task AddVehicleAsync(Vehicle vehicle)
        {
            await redisRepository.SaveVehicleAsync(vehicle);
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation(
                    "Vehicle added: {VehicleID} ({Type}), capacity {Capacity}, speed {Speed} km/h.",
                    vehicle.VehicleID, vehicle.Type, vehicle.Capacity, vehicle.Speed
                );
            }
        }
    }
}