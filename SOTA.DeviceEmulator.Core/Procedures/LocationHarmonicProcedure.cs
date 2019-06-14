using System;
using GeoAPI.Geometries;
using NetTopologySuite;

namespace SOTA.DeviceEmulator.Core.Procedures
{
    // This procedure generates location data. All points belong to a circle on the earth's surface.
    public class LocationHarmonicProcedure : IProcedure<IPoint>
    {
        // Central point of circle trajectory.
        private const double ZeroLatitude = 36.438685;
        private const double ZeroLongitude = 28.211944;

        // Trajectory circle radius on the earth's surface.
        private double TrajectoryRadius => 3;
        private double AngleCoefficient => TrajectoryRadius / Math.Sqrt(TrajectoryRadius * TrajectoryRadius + EarthGeometry.EarthRadius * EarthGeometry.EarthRadius);
        protected TimeSpan Period => TimeSpan.FromHours(1);

        public IPoint GetValue(TimeSpan elapsedTime)
        {
            var phase = OscillationMath.CalculatePhase(elapsedTime, Period);

            var latitude = ZeroLatitude + OscillationMath.RadianToDegree(AngleCoefficient * Math.Cos(phase));
            var longitude = ZeroLongitude + OscillationMath.RadianToDegree(AngleCoefficient * Math.Sin(phase));

            // Order of lat and lon is backwards to typical
            var point = EarthGeometry.GeometryFactory.CreatePoint(new Coordinate(longitude, latitude));

            return point;
        }
    }
}
