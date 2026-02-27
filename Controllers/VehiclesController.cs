using EvacApiProblem.DTOs;
using EvacApiProblem.Models;
using EvacApiProblem.Services;
using Microsoft.AspNetCore.Mvc;

namespace EvacApiProblem.Controllers
{
    /// <summary>
    /// Vehicles Controller
    /// </summary>
    [ApiController]
    [Route("api/vehicles")]
    public class VehiclesController(IVehicleService vehicleService, ILogger<VehiclesController> logger) : ControllerBase
    {

        /// <summary>
        /// Adds new vehicle
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<VehicleResponse>> AddVehicle([FromBody] CreateVehicleRequest request)
        {
            try
            {
                // Create new vehicle
                var newVehicle = new Vehicle
                {
                    VehicleID = request.VehicleID,
                    Type = request.Type,
                    Capacity = request.Capacity,
                    Speed = request.Speed,
                    LocationCoordinates = new Location
                    {
                        Latitude = request.LocationCoordinates.Latitude,
                        Longitude = request.LocationCoordinates.Longitude
                    }
                };

                await vehicleService.AddVehicleAsync(newVehicle);

                // Mapping to response DTO
                var response = new VehicleResponse
                {
                    VehicleID = newVehicle.VehicleID,
                    Type = newVehicle.Type,
                    Capacity = newVehicle.Capacity,
                    Speed = newVehicle.Speed,
                    LocationCoordinates = new LocationDto
                    {
                        Latitude = newVehicle.LocationCoordinates.Latitude,
                        Longitude = newVehicle.LocationCoordinates.Longitude
                    }
                };

                return CreatedAtAction(nameof(AddVehicle), new { id = response.VehicleID }, response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding vehicle.");
                return StatusCode(500, new { error = "Failed to add vehicle.", details = ex.Message });
            }
        }
    }
}