using System.ComponentModel.DataAnnotations;

namespace EvacApiProblem.DTOs
{
    /// <summary>
    /// Request to add evacuation zone
    /// </summary>
    public class CreateEvacuationZoneRequest
    {
        [Required(ErrorMessage = "ZoneID is required.")]
        public string ZoneID { get; set; } = string.Empty;

        [Required(ErrorMessage = "LocationCoordinates is required.")]
        public LocationDto LocationCoordinates { get; set; } = new LocationDto();

        [Range(1, int.MaxValue, ErrorMessage = "NumberOfPeople must be at least 1.")]
        public int NumberOfPeople { get; set; }

        [Range(1, 5, ErrorMessage = "UrgencyLevel must be between 1 (low) and 5 (high).")]
        public int UrgencyLevel { get; set; }
    }
}