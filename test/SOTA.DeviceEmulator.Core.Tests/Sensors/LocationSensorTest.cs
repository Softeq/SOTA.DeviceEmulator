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
        private static IPoint _functionResult;

        public static IEnumerable<object[]> GenerateData()
        {
            var measureStartTime = new DateTime(2019, 10, 10);
            var measureTimeValues = new List<DateTime>
            {
                measureStartTime + TimeSpan.FromMinutes(10),
                measureStartTime + TimeSpan.FromMinutes(20),
                measureStartTime + TimeSpan.FromMinutes(30),
                measureStartTime + TimeSpan.FromMinutes(40),
                measureStartTime + TimeSpan.FromMinutes(50),
            };
            var functionMock = new Mock<ILocationFunction>(MockBehavior.Strict);
            var locationSensorPropertiesMock = new Mock<ILocationSensorOptions>(MockBehavior.Strict);
            var locationSensor = new LocationSensor(locationSensorPropertiesMock.Object)
            {
                Function = functionMock.Object
            };

            functionMock.Setup(i => i.Speed).Returns(0);
            locationSensorPropertiesMock.Setup(i => i.SpeedMean).Returns(5);
            locationSensorPropertiesMock.Setup(i => i.SpeedDeviation).Returns(1);

            _functionResult = EarthGeometry.GeometryFactory.CreatePoint(new Coordinate(0, 0));

            measureTimeValues.ForEach(
                time =>
                functionMock.Setup(i => i.GetValue(time)).Returns(_functionResult));

            var testData = measureTimeValues.Select(time =>
            {
                return new object[]
                {
                    Enumerable.Range(0, 20).Select(i => locationSensor.GetValue(time)).ToList()
                };
            });

            return testData;
        }

        [Theory]
        [MemberData(nameof(GenerateData))]
        public void Returns_ValidRandomValue_When_RangePassed(IEnumerable<IPoint> points)
        {
            foreach (var point in points)
            {
                point.X.Should().Be(_functionResult.X);
                point.Y.Should().Be(_functionResult.Y);
            }
        }
    }
}
