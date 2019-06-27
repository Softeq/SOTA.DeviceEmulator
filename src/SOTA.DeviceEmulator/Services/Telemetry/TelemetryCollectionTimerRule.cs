using System;
using Caliburn.Micro;
using EnsureThat;
using SOTA.DeviceEmulator.Services.Infrastructure.Jobs;

namespace SOTA.DeviceEmulator.Services.Telemetry
{
    public class TelemetryCollectionTimerRule : INotificationTimerRule
    {
        private readonly IEventAggregator _eventAggregator;

        public TelemetryCollectionTimerRule(IEventAggregator eventAggregator)
        {
            _eventAggregator = Ensure.Any.IsNotNull(eventAggregator, nameof(eventAggregator));
        }

        public TimeSpan Period => TimeSpan.FromSeconds(1);

        public void Trigger()
        {
            var notification = new TelemetryCollectionRequested();
            _eventAggregator.PublishOnBackgroundThread(notification);
        }
    }
}