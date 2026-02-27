using System.ComponentModel.DataAnnotations;

namespace EvacApiProblem.DTOs
{
    /// <summary>
    /// Request to add vehicle
    /// </summary>
    public class CreateVehicleRequest
    {
        [Required(ErrorMessage = "VehicleID is required.")]
        public string VehicleID { get; set; } = string.Empty;

        [Required(ErrorMessage = "Type is required.")]
        public string Type { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be at least 1.")]
        public int Capacity { get; set; }

        [Range(0.1, double.MaxValue, ErrorMessage = "Speed must be greater than 0.")]
        public double Speed { get; set; }

        [Required(ErrorMessage = "LocationCoordinates is required.")]
        public LocationDto LocationCoordinates { get; set; } = new LocationDto();
    }
}