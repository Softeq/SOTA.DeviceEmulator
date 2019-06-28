using System;
using MediatR;
using SOTA.DeviceEmulator.Core;

namespace SOTA.DeviceEmulator.Services.Telemetry
{
    public class TelemetryCollected : INotification
    {
        public TelemetryCollected(DeviceTelemetry value, TimeSpan sessionTime)
        {
            Value = value;
            SessionTime = sessionTime;
        }

        public DeviceTelemetry Value { get; }

        public TimeSpan SessionTime { get; }
    }
}
