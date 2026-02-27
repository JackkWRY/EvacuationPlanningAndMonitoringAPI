using EvacApiProblem.Models;

namespace EvacApiProblem.Repositories
{
    /// <summary>
    /// Interface for Redis repository
    /// </summary>
    public interface IRedisRepository
    {
        Task SaveZoneAsync(EvacuationZone zone);                // Save evacuation zone to Redis
        Task<EvacuationZone?> GetZoneByIdAsync(string zoneId);  // Get evacuation zone by ID from Redis
        Task<List<EvacuationZone>> GetAllZonesAsync();          // Get all evacuation zones from Redis
        Task SaveVehicleAsync(Vehicle vehicle);                 // Save vehicle to Redis
        Task<Vehicle?> GetVehicleByIdAsync(string vehicleId);   // Get vehicle by ID from Redis
        Task<List<Vehicle>> GetAllVehiclesAsync();              // Get all vehicles from Redis
        Task SaveStatusAsync(EvacuationStatus status);          // Save status to Redis
        Task<EvacuationStatus?> GetStatusAsync(string zoneId);  // Get status by zone ID from Redis
        Task<List<EvacuationStatus>> GetAllStatusesAsync();     // Get all statuses from Redis
        Task ClearAllDataAsync();                               // Clear all data from Redis
        Task<bool> AcquireLockAsync(string lockKey, TimeSpan expiration); // Acquire lock
        Task ReleaseLockAsync(string lockKey);                  // Release lock
    }
}