using System;
using EnsureThat;
using Serilog;

namespace SOTA.DeviceEmulator.Services.Infrastructure.Jobs
{
    internal class NotificationTimer
    {
        private readonly ILogger _logger;
        private readonly INotificationTimerRule _rule;
        private DateTime? _lastExecutionTime;

        public NotificationTimer(INotificationTimerRule rule, ILogger logger)
        {
            _logger = Ensure.Any.IsNotNull(logger, nameof(logger));
            _rule = Ensure.Any.IsNotNull(rule, nameof(rule));
        }

        public string Name => _rule.GetType().Name;

        /// <returns>Estimated delay until next execution.</returns>
        public TimeSpan TriggerIfScheduled(DateTime now)
        {
            var estimatedExecutionTime = _lastExecutionTime == null ? now : _lastExecutionTime.Value + _rule.Period;
            if (estimatedExecutionTime > now)
            {
                return now - estimatedExecutionTime;
            }
            _rule.Trigger();
            _logger.Verbose("Timer {TimerName} was triggered.", Name);
            _lastExecutionTime = now;
            return _rule.Period;
        }
    }
}