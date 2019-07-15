using SOTA.DeviceEmulator.Core.Telemetry;

namespace SOTA.DeviceEmulator.Core.Configuration
{
    public interface IDeviceConfiguration
    {
        ITransmissionOptions Transmission { get; }

        ILocationSensorOptions Location { get; }

        IPulseSensorOptions Pulse { get; }
    }
}
