using System;
using System.Collections.Generic;
using SOTA.DeviceEmulator.Core.Procedures;
using Xunit;

namespace SOTA.DeviceEmulator.Core.Tests.Procedures
{
    public class PulseHarmonicProcedureTest
    {
        private const int MiddleValue = 70;
        private const int Amplitude = 10;

        private static readonly TimeSpan Period = TimeSpan.FromSeconds(5);
        private static readonly TimeSpan ZeroPeriod = TimeSpan.FromSeconds(0);
        private static readonly TimeSpan QuarterPeriod = TimeSpan.FromMilliseconds(Period.TotalMilliseconds / 4);
        private static readonly TimeSpan HalfPeriod = TimeSpan.FromMilliseconds(Period.TotalMilliseconds / 2);
        private static readonly TimeSpan ThreeQuarterPeriod = TimeSpan.FromMilliseconds(Period.TotalMilliseconds * 3 / 4);

        public static IEnumerable<object[]> Data =>
            new List<object[]>
            {
                new object[] { ZeroPeriod, MiddleValue },
                new object[] { QuarterPeriod, MiddleValue + Amplitude },
                new object[] { HalfPeriod, MiddleValue },
                new object[] { ThreeQuarterPeriod, MiddleValue - Amplitude },
                new object[] { Period, MiddleValue }
            };

        private readonly PulseHarmonicProcedure _procedure;


        public PulseHarmonicProcedureTest()
        {
            _procedure = new PulseHarmonicProcedure();
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void Returns_ValidValue_When_TimePassed(TimeSpan elapsedTime, double expectedValue)
        {
            var value = _procedure.GetValue(elapsedTime);

            Assert.Equal(expectedValue, value);
        }
    }
}
