using System;

namespace SOTA.DeviceEmulator.Core.Procedures
{
    public class PulseHarmonicProcedure : IProcedure<double>
    {
        protected double MiddleValue => 70;
        protected double Amplitude => 10;
        protected TimeSpan Period => TimeSpan.FromSeconds(5);

        public double GetValue(TimeSpan elapsedTime)
        {
            var phase = OscillationMath.CalculatePhase(elapsedTime, Period);
            return MiddleValue + Amplitude * Math.Sin(phase);
        }
    }
}
