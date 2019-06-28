using System;
using SOTA.DeviceEmulator.Core.Sensors;

namespace SOTA.DeviceEmulator.Core
{
    public interface IDeviceState
    {
        IPulseSensorOptions PulseSensorOptions { get; }

        ILocationSensorOptions LocationSensorOptions { get; }

        bool IsTransmissionEnabled { get; set; }

        int TransmissionPeriod { get; set; }

        TimeSpan SessionTime { get; set; }
    }
}
