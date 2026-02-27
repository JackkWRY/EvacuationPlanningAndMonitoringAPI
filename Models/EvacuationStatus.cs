namespace EvacApiProblem.Models
{
    /// <summary>
    /// Represents the evacuation status
    /// </summary>
    public class EvacuationStatus
    {
        public string ZoneID { get; set; } = string.Empty;
        public int TotalEvacuated { get; set; }
        public int RemainingPeople { get; set; }
        public string? LastVehicleUsed { get; set; }
    }
}