using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using SOTA.DeviceEmulator.Core.Telemetry;
using SOTA.DeviceEmulator.Core.Telemetry.TimeFunctions;
using Xunit;

namespace SOTA.DeviceEmulator.Core.Tests.Telemetry
{
    public class PulseSensorTest
    {
        private const double ProcedureFunctionResult = 70;
        private const int NoiseFactor = 5;

        public static IEnumerable<object[]> GenerateData()
        {
            var measuringTime = new DateTime(2019, 10, 10);
            var functionMock = new Mock<ITimeFunction<double>>(MockBehavior.Strict);
            functionMock.Setup(i => i.GetValue(measuringTime)).Returns(ProcedureFunctionResult);

            var pulseSensorOptions = new Mock<IPulseSensorOptions>();
            pulseSensorOptions.Setup(i => i.NoiseFactor).Returns(NoiseFactor);
            pulseSensorOptions.Setup(i => i.Function).Returns(functionMock.Object);

            var pulseSensor = new PulseSensor(pulseSensorOptions.Object);

            var testData = new List<object[]>
            {
                new object[]
                {
                    Enumerable.Range(0, 20).Select(i => pulseSensor.GetValue(measuringTime)).ToList()
                }
            };

            return testData;
        }

        [Theory]
        [MemberData(nameof(GenerateData))]
        public void Returns_ValidRandomValue_When_RangePassed(IEnumerable<int> pulses)
        {
            foreach (var pulse in pulses)
            {
                pulse.Should().BeInRange((int)ProcedureFunctionResult - NoiseFactor, (int)ProcedureFunctionResult + NoiseFactor);
            }
        }
    }
}
