
namespace SOTA.DeviceEmulator.Core
{
    public interface IDevice
    {
        string DisplayName { get; }
        bool IsConnected { get; }
        void Connect(DeviceConnectionMetadata connectionMetadata);
        void Disconnect();
        DeviceMetadata Metadata { get; }
        DeviceTelemetry ReportTelemetry();
    }
}