using MediatR;
using SOTA.DeviceEmulator.Core;

namespace SOTA.DeviceEmulator.Services.Telemetry
{
    public class TelemetryCollected : INotification
    {
        public TelemetryCollected(DeviceTelemetry value)
        {
            Value = value;
        }

        public DeviceTelemetry Value { get; }
    }
}
