using EnsureThat;
using MediatR;
using SOTA.DeviceEmulator.Core;
using SOTA.DeviceEmulator.Core.Telemetry;

namespace SOTA.DeviceEmulator.Services.Telemetry
{
    public class TelemetryPublished : INotification
    {
        public TelemetryPublished(DeviceTelemetry telemetry)
        {
            Telemetry = Ensure.Any.IsNotNull(telemetry, nameof(telemetry));
        }

        public DeviceTelemetry Telemetry { get; }
    }
}