using System;
using GeoAPI.Geometries;

namespace SOTA.DeviceEmulator.Core.Sensors.TimeFunctions
{
    // This function generates location data. All points belong to a circle on the earth's surface.
    public class LocationHarmonicFunction : ILocationFunction
    {
        // Trajectory circle radius on the earth's surface in kilometers.
        public static double TrajectoryRadius => 3;
        public string DisplayName => "Harmonic";

        // Central point of circle trajectory. Yellowstone National Park, WY, USA.
        public static double ZeroLatitude => 44.548004;
        public static double ZeroLongitude => -110.675640;

        // Speed in kilometers per hour.
        public double Speed { get; set; } = 5;

        private DateTime _lastRequestTime = DateTime.MinValue;
        private double _lastRequestPhase;

        private double AngleCoefficient => TrajectoryRadius / Math.Sqrt(TrajectoryRadius * TrajectoryRadius + EarthGeometry.EarthRadius * EarthGeometry.EarthRadius);
        private TimeSpan Period => TimeSpan.FromHours(2 * Math.PI * TrajectoryRadius / Speed);

        public IPoint GetValue(DateTime time)
        {
            var elapsedTime = time - _lastRequestTime;
            var phase = OscillationMath.CalculatePhase(elapsedTime, Period);

             _lastRequestTime = time;
             phase += _lastRequestPhase;
             _lastRequestPhase = phase;

            var latitude = ZeroLatitude + OscillationMath.RadianToDegree(AngleCoefficient * Math.Cos(phase));
            var longitude = ZeroLongitude + OscillationMath.RadianToDegree(AngleCoefficient * Math.Sin(phase));

            // Order of lat and lon is backwards to typical
            var point = EarthGeometry.GeometryFactory.CreatePoint(new Coordinate(longitude, latitude));

            return point;
        }
    }
}
