using SOTA.DeviceEmulator.Core.Telemetry.TimeFunctions;

namespace SOTA.DeviceEmulator.Core.Telemetry
{
    public interface IPulseSensorOptions
    {
        int NoiseFactor { get; set; }
        ITimeFunction<double> Function { get; set; }
    }
}