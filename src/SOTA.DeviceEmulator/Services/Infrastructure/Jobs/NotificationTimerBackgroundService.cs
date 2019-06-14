using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace SOTA.DeviceEmulator.Services.Infrastructure.Jobs
{
    public class NotificationTimerBackgroundService : BackgroundService
    {
        private readonly IClock _clock;
        private readonly ILogger _logger;
        private readonly List<NotificationTimer> _timers;

        public NotificationTimerBackgroundService(ILogger logger, IClock clock,
            IEnumerable<INotificationTimerRule> timerRules)
        {
            Ensure.Any.IsNotNull(clock, nameof(clock));
            Ensure.Any.IsNotNull(logger, nameof(logger));
            Ensure.Any.IsNotNull(timerRules, nameof(timerRules));

            _logger = logger;
            _clock = clock;
            _timers = timerRules.Select(rule => new NotificationTimer(rule)).ToList();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.Information("Notification timer is started.");
            if (!_timers.Any())
            {
                _logger.Warning("No timers registered.");
                return;
            }

            // Delay to let UI subscribe to changes
            await SafeDelay(TimeSpan.FromSeconds(1), stoppingToken);
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = _clock.UtcNow;

                TimeSpan TriggerSafe(NotificationTimer timer)
                {
                    try
                    {
                        return timer.TriggerIfScheduled(now);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "Failed to trigger timer {TimerName}.", timer.Name);
                        return TimeSpan.FromMinutes(1);
                    }
                }

                var delay = _timers.Select(TriggerSafe).Min();
                if (delay <= TimeSpan.Zero)
                {
                    continue;
                }

                await SafeDelay(delay, stoppingToken);
            }

            _logger.Information("Notification timer is stopped.");
        }

        private async Task SafeDelay(TimeSpan delay, CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay(delay, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // Do nothing
            }
        }
    }
}