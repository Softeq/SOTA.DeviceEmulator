using System;
using System.Linq;
using GeoAPI.Geometries;
using Moq;
using SOTA.DeviceEmulator.Core.Procedures;
using SOTA.DeviceEmulator.Core.Sensors;
using SOTA.DeviceEmulator.Core.Tests.Stubs;
using Xunit;

namespace SOTA.DeviceEmulator.Core.Tests.Sensors
{
    public class LocationSensorTest
    {
        // Noise step in kilometers.
        private const double NoiseStep = 0.1;

        private readonly IPoint _procedureResult;
        private readonly LocationSensor _locationSensor;

        public LocationSensorTest()
        {
            var procedure = new Mock<IProcedure<IPoint>>(MockBehavior.Strict);
            _procedureResult = EarthGeometry.GeometryFactory.CreatePoint(new Coordinate(0, 0));

            procedure.Setup(i => i.GetValue(It.IsAny<TimeSpan>())).Returns(_procedureResult);

            var clock = new TestClock();

            _locationSensor = new LocationSensor(procedure.Object, clock);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(10)]
        public void Returns_ValidRandomValue_When_RangePassed(int noiseFactor)
        {
            const int valuesCount = 20;
            var results = new IPoint[valuesCount];

            for (var i = 0; i < valuesCount; i++)
            {
                results[i] = _locationSensor.GetValue(noiseFactor);
            }

            Assert.True(results.All(
                result =>
                    result.X <= _procedureResult.X + noiseFactor * NoiseStep &&
                    result.X >= _procedureResult.X - noiseFactor * NoiseStep &&
                    result.Y <= _procedureResult.Y + noiseFactor * NoiseStep &&
                    result.Y >= _procedureResult.Y - noiseFactor * NoiseStep));
        }
    }
}
