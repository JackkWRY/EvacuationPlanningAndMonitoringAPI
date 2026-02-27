namespace EvacApiProblem.DTOs
{
    /// <summary>
    /// Response for evacuation plan
    /// </summary>
    public class EvacuationPlanResponse
    {
        public string ZoneID { get; set; } = string.Empty;
        public string VehicleID { get; set; } = string.Empty;
        public string ETA { get; set; } = string.Empty;
        public int NumberOfPeople { get; set; }
    }
}