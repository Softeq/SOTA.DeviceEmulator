using FluentValidation.Results;
using SOTA.DeviceEmulator.Core.Configuration;
using SOTA.DeviceEmulator.Core.Telemetry;

namespace SOTA.DeviceEmulator.Core
{
    public interface IDevice
    {
        string DisplayName { get; }
        IDeviceConfiguration Configuration { get; }
        bool IsConnected { get; }
        ValidationResult Connect(DeviceMetadata metadata);
        void Disconnect();
        DeviceInformation Information { get; }
        DeviceTelemetryReport GetTelemetryReport();
        bool TryGetChangedConfiguration(out DeviceConfiguration deviceConfiguration);
    }
}