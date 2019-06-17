using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GeoAPI.Geometries;
using Moq;
using SOTA.DeviceEmulator.Core.Sensors;
using SOTA.DeviceEmulator.Core.Sensors.TimeFunctions;
using Xunit;

namespace SOTA.DeviceEmulator.Core.Tests.Sensors
{
    public class LocationSensorTest
    {
        private static DateTime _measuringTime;
        private static IPoint _procedureResult;
        private static LocationSensor _locationSensor;

        public static IEnumerable<object[]> GenerateData()
        {
            _measuringTime = new DateTime(2019, 10, 10);
            var procedure = new Mock<ITimeFunction<IPoint>>(MockBehavior.Strict);

            _procedureResult = EarthGeometry.GeometryFactory.CreatePoint(new Coordinate(0, 0));
            procedure.Setup(i => i.GetValue(_measuringTime)).Returns(_procedureResult);
            _locationSensor = new LocationSensor(procedure.Object);

            var noiseFactors = new[] {1, 2, 3, 4, 5, 10};
            var testData = noiseFactors.Select(factor =>
            {
                _locationSensor.NoiseFactor = factor;
                return new object[]
                {
                    factor,
                    Enumerable.Range(0, 20).Select(i => _locationSensor.GetValue(_measuringTime)).ToList()
                };
            });

            return testData;
        }

        [Theory]
        [MemberData(nameof(GenerateData))]
        public void Returns_ValidRandomValue_When_RangePassed(int noiseFactor, IEnumerable<IPoint> points)
        {
            foreach (var point in points)
            {
                point.X.Should().BeInRange(
                    _procedureResult.X - noiseFactor * LocationSensor.NoiseStep,
                    _procedureResult.X + noiseFactor * LocationSensor.NoiseStep);
                point.Y.Should().BeInRange(
                    _procedureResult.Y - noiseFactor * LocationSensor.NoiseStep,
                    _procedureResult.Y + noiseFactor * LocationSensor.NoiseStep);
            }
        }
    }
}
