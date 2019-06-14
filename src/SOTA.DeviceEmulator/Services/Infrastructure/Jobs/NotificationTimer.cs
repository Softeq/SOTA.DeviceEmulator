using System;
using Caliburn.Micro;
using EnsureThat;

namespace SOTA.DeviceEmulator.Services.Infrastructure.Jobs
{
    internal class NotificationTimer
    {
        private readonly INotificationTimerRule _rule;
        private DateTime? _lastExecutionTime;

        public NotificationTimer(INotificationTimerRule rule)
        {
            _rule = Ensure.Any.IsNotNull(rule, nameof(rule));
        }

        public string Name => _rule.GetType().Name;

        /// <returns>Estimated delay until next execution.</returns>
        public TimeSpan TriggerIfScheduled(IEventAggregator aggregator, DateTime now)
        {
            Ensure.Any.IsNotNull(aggregator, nameof(aggregator));

            var estimatedExecutionTime = _lastExecutionTime == null ? now : _lastExecutionTime.Value + _rule.Period;
            if (estimatedExecutionTime > now)
            {
                return now - estimatedExecutionTime;
            }
            var notification = _rule.CreateNotification();
            aggregator.PublishOnBackgroundThread(notification);
            _lastExecutionTime = now;
            return _rule.Period;
        }
    }
}