using System.ComponentModel.DataAnnotations;

namespace EvacApiProblem.DTOs
{
    /// <summary>
    /// Request to update evacuation status
    /// </summary>
    public class UpdateEvacuationStatusRequest
    {
        [Required(ErrorMessage = "ZoneID is required.")]
        public string ZoneID { get; set; } = string.Empty;

        [Required(ErrorMessage = "VehicleID is required.")]
        public string VehicleID { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "EvacuatedCount must be at least 1.")]
        public int EvacuatedCount { get; set; }
    }
}