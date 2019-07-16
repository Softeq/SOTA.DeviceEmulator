using System;
using Caliburn.Micro;
using EnsureThat;
using SOTA.DeviceEmulator.Core;
using SOTA.DeviceEmulator.Services.Infrastructure.Jobs;

namespace SOTA.DeviceEmulator.Services.Telemetry
{
    public class TelemetryCollectionTimerRule : INotificationTimerRule
    {
        private readonly IDevice _device;
        private readonly IEventAggregator _eventAggregator;

        public TelemetryCollectionTimerRule(IEventAggregator eventAggregator, IDevice device)
        {
            _device = Ensure.Any.IsNotNull(device, nameof(device));
            _eventAggregator = Ensure.Any.IsNotNull(eventAggregator, nameof(eventAggregator));
        }

        public bool IsEnabled => true;

        public TimeSpan Period => TimeSpan.FromSeconds(1);

        public void Trigger()
        {
            var report = _device.GetTelemetryReport();
            var collected = new TelemetryCollected(report.Telemetry, report.SessionTime);
            _eventAggregator.PublishOnUIThread(collected);
            if (!report.IsTelemetryPublished)
            {
                return;
            }
            var published = new TelemetryPublished(report.Telemetry);
            _eventAggregator.PublishOnBackgroundThread(published);
        }
    }
}