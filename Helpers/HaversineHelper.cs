using EvacApiProblem.Models;

namespace EvacApiProblem.Helpers
{
    /// <summary>
    /// Haversine helper class for calculating distances
    /// </summary>
    public static class HaversineHelper
    {
        private const double EarthRadiusKm = 6371.0;                        // Earth radius in kilometers

        /// <summary>
        /// Calculate distance between two points
        /// </summary>
        public static double CalculateDistance(Location loc1, Location loc2)
        {
            // Convert latitude and longitude from degrees to radians
            var lat1 = DegreesToRadians(loc1.Latitude);
            var lat2 = DegreesToRadians(loc2.Latitude);
            var lon1 = DegreesToRadians(loc1.Longitude);
            var lon2 = DegreesToRadians(loc2.Longitude);

            // Haversine formula
            var dLat = lat2 - lat1;
            var dLon = lon2 - lon1;
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var distance = EarthRadiusKm * c;

            return distance;
        }

        /// <summary>
        /// Convert degrees to radians
        /// </summary>
        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }
    }
}