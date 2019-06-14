using System;
using System.Linq;
using Moq;
using SOTA.DeviceEmulator.Core.Procedures;
using SOTA.DeviceEmulator.Core.Sensors;
using SOTA.DeviceEmulator.Core.Tests.Stubs;
using Xunit;

namespace SOTA.DeviceEmulator.Core.Tests.Sensors
{
    public class PulseSensorTest
    {
        private const double ProcedureResult = 70;
        private readonly PulseSensor _pulseSensor;

        public PulseSensorTest()
        {
            var procedure = new Mock<IProcedure<double>>(MockBehavior.Strict);
            procedure.Setup(i => i.GetValue(It.IsAny<TimeSpan>())).Returns(ProcedureResult);

            var clock = new TestClock();

            _pulseSensor = new PulseSensor(procedure.Object, clock);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(20)]
        [InlineData(30)]
        public void Returns_ValidRandomValue_When_RangePassed(int noiseFactor)
        {
            const int valuesCount = 20;
            var results = new int[valuesCount];

            for (var i = 0; i < valuesCount; i++)
            {
                results[i] = _pulseSensor.GetValue(noiseFactor);
            }

            Assert.True(results.All(
                result =>
                    result <= ProcedureResult + noiseFactor &&
                    result >= ProcedureResult - noiseFactor));
        }
    }
}
