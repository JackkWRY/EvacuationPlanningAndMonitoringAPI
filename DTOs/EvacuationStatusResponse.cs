namespace EvacApiProblem.DTOs
{
    /// <summary>
    /// Response for evacuation status
    /// </summary>
    public class EvacuationStatusResponse
    {
        public string ZoneID { get; set; } = string.Empty;
        public int TotalEvacuated { get; set; }
        public int RemainingPeople { get; set; }
        public string? LastVehicleUsed { get; set; }
    }
}