namespace EvacApiProblem.Models
{
    /// <summary>
    /// Represents the vehicle for evacuation
    /// </summary>
    public class Vehicle
    {
        public string VehicleID { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public string Type { get; set; } = string.Empty;
        public Location LocationCoordinates { get; set; } = new Location();
        public double Speed { get; set; }
    }
}