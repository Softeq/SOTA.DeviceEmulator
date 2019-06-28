using EnsureThat;
using SOTA.DeviceEmulator.Core;
using SOTA.DeviceEmulator.Core.Sensors;
using SOTA.DeviceEmulator.ViewModels;

namespace SOTA.DeviceEmulator.Services
{
    public class DeviceState : IDeviceState
    {
        public DeviceState(
            StatusBarViewModel statusBarViewModel,
            SensorsViewModel sensorsViewModel)
        {
            Transmission = Ensure.Any.IsNotNull(statusBarViewModel, nameof(statusBarViewModel));
            Location = Ensure.Any.IsNotNull(sensorsViewModel, nameof(sensorsViewModel));
            Pulse = Ensure.Any.IsNotNull(sensorsViewModel, nameof(sensorsViewModel));
        }

        public ITransmissionOptions Transmission { get; }
        public ILocationSensorOptions Location { get; }
        public IPulseSensorOptions Pulse { get; }
    }
}
