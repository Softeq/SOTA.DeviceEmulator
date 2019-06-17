using System;

namespace SOTA.DeviceEmulator.Core.Sensors
{
    public class OscillationMath
    {
        public static double CalculatePhase(TimeSpan elapsedTime, TimeSpan period)
        {
            return elapsedTime.TotalSeconds / period.TotalSeconds * 2 * Math.PI % (2 * Math.PI);
        }

        public static double RadianToDegree(double radian)
        {
            return radian * (180 / Math.PI);
        }
    }
}
