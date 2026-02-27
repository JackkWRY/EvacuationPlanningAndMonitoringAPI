using EvacApiProblem.Models;

namespace EvacApiProblem.Services
{
    /// <summary>
    /// Interface for evacuation service
    /// </summary>
    public interface IEvacuationService
    {
        Task AddEvacuationZoneAsync(EvacuationZone zone);   // Add evacuation zone
        Task<List<EvacuationPlan>> GeneratePlanAsync();     // Generate evacuation plan
        Task<List<EvacuationStatus>> GetAllStatusesAsync(); // Get all evacuation statuses
        Task UpdateStatusAsync(string zoneId, string vehicleId, int evacuatedCount); // Update evacuation status
        Task ClearAllDataAsync();                           // Clear all data
    }
}