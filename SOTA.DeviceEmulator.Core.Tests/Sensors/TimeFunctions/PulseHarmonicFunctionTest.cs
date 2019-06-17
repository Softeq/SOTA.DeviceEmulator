using System;
using System.Collections.Generic;
using SOTA.DeviceEmulator.Core.Sensors.TimeFunctions;
using Xunit;

namespace SOTA.DeviceEmulator.Core.Tests.Sensors.TimeFunctions
{
    public class PulseHarmonicFunctionTest
    {
        private const int MiddleValue = 70;
        private const int Amplitude = 10;

        private static readonly TimeSpan ZeroPeriod = TimeSpan.FromSeconds(0);
        private static readonly TimeSpan QuarterPeriod = TimeSpan.FromMilliseconds(PulseHarmonicFunction.Period.TotalMilliseconds / 4);
        private static readonly TimeSpan HalfPeriod = TimeSpan.FromMilliseconds(PulseHarmonicFunction.Period.TotalMilliseconds / 2);
        private static readonly TimeSpan ThreeQuarterPeriod = TimeSpan.FromMilliseconds(PulseHarmonicFunction.Period.TotalMilliseconds * 3 / 4);

        public static IEnumerable<object[]> Data =>
            new List<object[]>
            {
                new object[] { DateTime.MinValue + ZeroPeriod, MiddleValue },
                new object[] { DateTime.MinValue + QuarterPeriod, MiddleValue + Amplitude },
                new object[] { DateTime.MinValue + HalfPeriod, MiddleValue },
                new object[] { DateTime.MinValue + ThreeQuarterPeriod, MiddleValue - Amplitude },
                new object[] { DateTime.MinValue + PulseHarmonicFunction.Period, MiddleValue }
            };

        private readonly PulseHarmonicFunction _function;


        public PulseHarmonicFunctionTest()
        {
            _function = new PulseHarmonicFunction();
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void Returns_ValidValue_When_TimePassed(DateTime measuringTime, double expectedValue)
        {
            var value = _function.GetValue(measuringTime);

            Assert.Equal(expectedValue, value);
        }
    }
}
