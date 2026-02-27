using EvacApiProblem.DTOs;
using EvacApiProblem.Services;
using Microsoft.AspNetCore.Mvc;

namespace EvacApiProblem.Controllers
{
    /// <summary>
    /// Evacuation Controller
    /// </summary>
    [ApiController]
    [Route("api/evacuations")]
    public class EvacuationsController(IEvacuationService evacuationService, ILogger<EvacuationsController> logger) : ControllerBase
    {

        /// <summary>
        /// Generates an evacuation plan based on urgency level
        /// </summary>
        [HttpPost("plan")]
        public async Task<ActionResult<IEnumerable<EvacuationPlanResponse>>> GeneratePlan()
        {
            try
            {
                var plans = await evacuationService.GeneratePlanAsync();

                // Mapping to response DTO
                var response = plans.Select(plan => new EvacuationPlanResponse
                {
                    ZoneID = plan.ZoneID,
                    VehicleID = plan.VehicleID,
                    ETA = $"{plan.ETA} minutes",
                    NumberOfPeople = plan.NumberOfPeople
                }).ToList();

                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning(ex, "Plan generation failed.");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error generating plan.");
                return StatusCode(500, new { error = "Failed to generate evacuation plan.", details = ex.Message });
            }
        }

        /// <summary>
        /// Gets the current evacuation status for all zones
        /// </summary>
        [HttpGet("status")]
        public async Task<ActionResult<IEnumerable<EvacuationStatusResponse>>> GetStatus()
        {
            try
            {
                var statuses = await evacuationService.GetAllStatusesAsync();

                // Mapping to response DTO
                var response = statuses.Select(s => new EvacuationStatusResponse
                {
                    ZoneID = s.ZoneID,
                    TotalEvacuated = s.TotalEvacuated,
                    RemainingPeople = s.RemainingPeople,
                    LastVehicleUsed = s.LastVehicleUsed
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting status.");
                return StatusCode(500, new { error = "Failed to get evacuation status.", details = ex.Message });
            }
        }

        /// <summary>
        /// Updates the evacuation status for specific zone
        /// </summary>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateEvacuationStatus([FromBody] UpdateEvacuationStatusRequest request)
        {
            try
            {
                await evacuationService.UpdateStatusAsync(request.ZoneID, request.VehicleID, request.EvacuatedCount);

                return Ok(new { message = "Evacuation status updated successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating status.");
                return StatusCode(500, new { error = "Failed to update evacuation status.", details = ex.Message });
            }
        }

        /// <summary>
        /// Clears all data
        /// </summary>
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearAllData()
        {
            try
            {
                await evacuationService.ClearAllDataAsync();

                return Ok(new { message = "All data cleared." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error clearing data.");
                return StatusCode(500, new { error = "Failed to clear evacuation data.", details = ex.Message });
            }
        }
    }
}