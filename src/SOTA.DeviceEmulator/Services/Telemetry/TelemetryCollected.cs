using System;
using MediatR;
using SOTA.DeviceEmulator.Core;

namespace SOTA.DeviceEmulator.Services.Telemetry
{
    public class TelemetryCollected : INotification
    {
        public TelemetryCollected(DeviceTelemetry telemetry, TimeSpan sessionTime)
        {
            Telemetry = telemetry;
            SessionTime = sessionTime;
        }

        public DeviceTelemetry Telemetry { get; }

        public TimeSpan SessionTime { get; }
    }
}
