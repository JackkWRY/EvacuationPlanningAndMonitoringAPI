namespace EvacApiProblem.DTOs
{
    /// <summary>
    /// Response for evacuation zone
    /// </summary>
    public class EvacuationZoneResponse
    {
        public string ZoneID { get; set; } = string.Empty;
        public LocationDto LocationCoordinates { get; set; } = new LocationDto();
        public int NumberOfPeople { get; set; }
        public int UrgencyLevel { get; set; }
    }
}