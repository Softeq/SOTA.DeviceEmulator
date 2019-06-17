using System;
using System.Collections.Generic;
using GeoAPI.Geometries;
using NetTopologySuite.Algorithm.Distance;
using SOTA.DeviceEmulator.Core.Sensors;
using SOTA.DeviceEmulator.Core.Sensors.TimeFunctions;
using Xunit;

namespace SOTA.DeviceEmulator.Core.Tests.Sensors.TimeFunctions
{
    public class LocationHarmonicFunctionTest
    {
        private readonly IPoint _zeroPoint = EarthGeometry.GeometryFactory.CreatePoint(new Coordinate(28.211944, 36.438685));

        private readonly LocationHarmonicFunction _locationFunction;

        // Used for rounding distance in km to 4 digits after a comma.
        private const double DistanceComparisonPrecision = 10000;

        // Trajectory circle radius on the earth's surface.
        private double TrajectoryRadius => 3;

        public static IEnumerable<object[]> Data =>
            new List<object[]>
            {
                new object[] { DateTime.MinValue + TimeSpan.FromHours(0) },
                new object[] { DateTime.MinValue + TimeSpan.FromHours(0.25) },
                new object[] { DateTime.MinValue + TimeSpan.FromHours(0.5) },
                new object[] { DateTime.MinValue + TimeSpan.FromHours(0.75) },
                new object[] { DateTime.MinValue + TimeSpan.FromHours(1) }
            };

        public LocationHarmonicFunctionTest()
        {
            _locationFunction = new LocationHarmonicFunction();
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void Returns_PointOnCircle_When_TimePassed(DateTime measuringTime)
        {
            var result = _locationFunction.GetValue(measuringTime);

            var distance = new PointPairDistance();
            distance.Initialize(result.Coordinate, _zeroPoint.Coordinate);

            var distanceInKm = RoundWithPrecision(EarthGeometry.ConvertAngleSizeToDistance(distance.Distance), DistanceComparisonPrecision);

            Assert.Equal(TrajectoryRadius, distanceInKm);
        }

        private double RoundWithPrecision(double value, double precision)
        {
            return Math.Round(value * precision, MidpointRounding.AwayFromZero) / precision;
        }
    }
}
