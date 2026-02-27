namespace EvacApiProblem.Models
{
    /// <summary>
    /// Represents the evacuation plan
    /// </summary>
    public class EvacuationPlan
    {
        public string ZoneID { get; set; } = string.Empty;
        public string VehicleID { get; set; } = string.Empty;
        public int ETA { get; set; }
        public int NumberOfPeople { get; set; }
    }
}