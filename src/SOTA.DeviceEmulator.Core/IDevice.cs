using SOTA.DeviceEmulator.Core.Sensors;

namespace SOTA.DeviceEmulator.Core
{
    public interface IDevice
    {
        DeviceTelemetry ReportTelemetry();
    }
}