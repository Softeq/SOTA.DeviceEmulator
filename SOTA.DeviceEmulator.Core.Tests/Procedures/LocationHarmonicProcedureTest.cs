using System;
using System.Collections.Generic;
using GeoAPI.Geometries;
using NetTopologySuite.Algorithm.Distance;
using SOTA.DeviceEmulator.Core.Procedures;
using Xunit;

namespace SOTA.DeviceEmulator.Core.Tests.Procedures
{
    public class LocationHarmonicProcedureTest
    {
        private readonly IPoint _zeroPoint = EarthGeometry.GeometryFactory.CreatePoint(new Coordinate(28.211944, 36.438685));

        private readonly LocationHarmonicProcedure _locationProcedure;

        private const double DistanceComparisonPrecision = 10000;

        // Trajectory circle radius on the earth's surface.
        private double TrajectoryRadius => 3;

        public static IEnumerable<object[]> Data =>
            new List<object[]>
            {
                new object[] { TimeSpan.FromHours(0) },
                new object[] { TimeSpan.FromHours(0.25) },
                new object[] { TimeSpan.FromHours(0.5) },
                new object[] { TimeSpan.FromHours(0.75) },
                new object[] { TimeSpan.FromHours(1) }
            };

        public LocationHarmonicProcedureTest()
        {
            _locationProcedure = new LocationHarmonicProcedure();
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void Returns_PointOnCircle_When_TimePassed(TimeSpan elapsedTime)
        {
            var result = _locationProcedure.GetValue(elapsedTime);

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
