using SOTA.DeviceEmulator.Core.Sensors;

namespace SOTA.DeviceEmulator.Core
{
    public interface IDeviceState
    {
        ITransmissionOptions Transmission { get; }

        ILocationSensorOptions Location { get; }

        IPulseSensorOptions Pulse { get; }
    }
}
