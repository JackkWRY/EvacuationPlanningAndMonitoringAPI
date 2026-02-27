using EvacApiProblem.Models;

namespace EvacApiProblem.Services
{
    /// <summary>
    /// Interface for vehicle service
    /// </summary>
    public interface IVehicleService
    {
        Task AddVehicleAsync(Vehicle vehicle);      // Add vehicle
    }
}