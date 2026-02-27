namespace EvacApiProblem.Models
{
    /// <summary>
    /// Represents the evacuation zone
    /// </summary>
    public class EvacuationZone
    {
        public string ZoneID { get; set; } = string.Empty;
        public Location LocationCoordinates { get; set; } = new Location();
        public int NumberOfPeople { get; set; }
        public int UrgencyLevel { get; set; }
    }
}