using MediatR;

namespace SOTA.DeviceEmulator.Services.Telemetry
{
    public class TelemetryCollected : INotification
    {
        public TelemetryCollected(int value)
        {
            Value = value;
        }

        public int Value { get; }
    }
}
