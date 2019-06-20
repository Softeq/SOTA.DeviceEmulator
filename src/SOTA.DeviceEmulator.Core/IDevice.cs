using SOTA.DeviceEmulator.Core.Sensors;

namespace SOTA.DeviceEmulator.Core
{
    public interface IDevice
    {
        PulseSensor PulseSensor { get; }
        LocationSensor LocationSensor { get; }
        DeviceTelemetry ReportTelemetry();
    }
}