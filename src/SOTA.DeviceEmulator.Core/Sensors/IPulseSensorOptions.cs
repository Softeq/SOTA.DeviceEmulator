using SOTA.DeviceEmulator.Core.Sensors.TimeFunctions;

namespace SOTA.DeviceEmulator.Core.Sensors
{
    public interface IPulseSensorOptions
    {
        int PulseNoiseFactor { get; }
        ITimeFunction<double> PulseFunction { get; }
    }
}