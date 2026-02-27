using EvacApiProblem.DTOs;
using EvacApiProblem.Models;
using EvacApiProblem.Services;
using Microsoft.AspNetCore.Mvc;

namespace EvacApiProblem.Controllers
{
    /// <summary>
    /// Evacuation Zones Controller
    /// </summary>
    [ApiController]
    [Route("api/evacuation-zones")]
    public class EvacuationZonesController(IEvacuationService evacuationService, ILogger<EvacuationZonesController> logger) : ControllerBase
    {

        /// <summary>
        /// Adds new evacuation zone
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<EvacuationZoneResponse>> AddZone([FromBody] CreateEvacuationZoneRequest request)
        {
            try
            {
                // Create new evacuation zone
                var newZone = new EvacuationZone
                {
                    ZoneID = request.ZoneID,
                    LocationCoordinates = new Location
                    {
                        Latitude = request.LocationCoordinates.Latitude,
                        Longitude = request.LocationCoordinates.Longitude
                    },
                    NumberOfPeople = request.NumberOfPeople,
                    UrgencyLevel = request.UrgencyLevel
                };

                await evacuationService.AddEvacuationZoneAsync(newZone);

                // Mapping to response DTO
                var response = new EvacuationZoneResponse
                {
                    ZoneID = newZone.ZoneID,
                    LocationCoordinates = new LocationDto
                    {
                        Latitude = newZone.LocationCoordinates.Latitude,
                        Longitude = newZone.LocationCoordinates.Longitude
                    },
                    NumberOfPeople = newZone.NumberOfPeople,
                    UrgencyLevel = newZone.UrgencyLevel
                };

                return CreatedAtAction(nameof(AddZone), new { id = response.ZoneID }, response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding evacuation zone.");
                return StatusCode(500, new { error = "Failed to add evacuation zone.", details = ex.Message });
            }
        }
    }
}