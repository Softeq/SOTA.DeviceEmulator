using System;
using EnsureThat;

namespace SOTA.DeviceEmulator.Core.Telemetry
{
    public class DeviceTelemetryReport
    {
        public DeviceTelemetryReport(DeviceTelemetry telemetry, bool isTelemetryPublished, TimeSpan sessionTime)
        {
            Telemetry = Ensure.Any.IsNotNull(telemetry, nameof(telemetry));
            IsTelemetryPublished = isTelemetryPublished;
            SessionTime = Ensure.Comparable.IsGte(sessionTime, TimeSpan.Zero, nameof(sessionTime));
        }

        public DeviceTelemetry Telemetry { get; }

        public bool IsTelemetryPublished { get; }

        public TimeSpan SessionTime { get; }
    }
}
