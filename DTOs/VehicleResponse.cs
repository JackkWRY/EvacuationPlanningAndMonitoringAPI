namespace EvacApiProblem.DTOs
{
    /// <summary>
    /// Response for vehicle
    /// </summary>
    public class VehicleResponse
    {
        public string VehicleID { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public double Speed { get; set; }
        public LocationDto LocationCoordinates { get; set; } = new LocationDto();
    }
}