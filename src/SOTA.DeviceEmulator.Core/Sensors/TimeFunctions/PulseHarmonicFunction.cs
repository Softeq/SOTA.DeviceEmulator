using System;

namespace SOTA.DeviceEmulator.Core.Sensors.TimeFunctions
{
    public class PulseHarmonicFunction : ITimeFunction<double>
    {
        public static TimeSpan Period => TimeSpan.FromSeconds(5);

        protected double MiddleValue => 70;
        protected double Amplitude => 10;

        public double GetValue(DateTime time)
        {
            var elapsedTime = time - DateTime.MinValue;
            var phase = OscillationMath.CalculatePhase(elapsedTime, Period);
            return MiddleValue + Amplitude * Math.Sin(phase);
        }
    }
}
