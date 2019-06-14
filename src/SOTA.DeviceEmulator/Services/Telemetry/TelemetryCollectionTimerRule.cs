using System;
using MediatR;
using SOTA.DeviceEmulator.Services.Infrastructure.Jobs;

namespace SOTA.DeviceEmulator.Services.Telemetry
{
    public class TelemetryCollectionTimerRule : INotificationTimerRule
    {
        public TimeSpan Period => TimeSpan.FromSeconds(5);

        public INotification CreateNotification()
        {
            return new TelemetryCollectionRequested();
        }
    }
}