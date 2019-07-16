using System;
using System.Collections.Concurrent;
using System.Threading;
using Caliburn.Micro;
using EnsureThat;
using MediatR;
using Serilog;
using SOTA.DeviceEmulator.Core;
using SOTA.DeviceEmulator.Core.Configuration;

namespace SOTA.DeviceEmulator.Services
{
    internal class EventPublisher : IEventPublisher
    {
        private readonly ILogger _logger;
        private readonly IEventAggregator _eventAggregator;
        private readonly ConcurrentDictionary<Type, Timer> _timers;

        public EventPublisher(ILogger logger, IEventAggregator eventAggregator)
        {
            _logger = Ensure.Any.IsNotNull(logger, nameof(logger)).ForContext(GetType());
            _eventAggregator = Ensure.Any.IsNotNull(eventAggregator, nameof(eventAggregator));
            _timers = new ConcurrentDictionary<Type, Timer>();
        }

        public void Publish<T>(T @event) where T : class
        {
            Ensure.Any.IsNotNull(@event, nameof(@event));

            switch (@event)
            {
                case DeviceConfigurationChanged configurationChanged:
                    Debounce(configurationChanged, TimeSpan.FromSeconds(5), PublishOnBackgroundThread);
                    break;
                default:
                    PublishOnUIThread(@event);
                    break;
            }
        }

        private void Debounce<T>(T @event, TimeSpan delay, Action<T> publish) where T : class
        {
            void OnTimer(object timerEvent)
            {
                publish((T)timerEvent);
                var removed = _timers.TryRemove(timerEvent.GetType(), out var removedTimer);
                if (removed)
                {
                    removedTimer.Dispose();
                }
            }
            var created = false;
            var timer = _timers.GetOrAdd(
                @event.GetType(),
                type =>
                {
                    created = true;
                    return new Timer(OnTimer, @event, delay, TimeSpan.FromMilliseconds(-1));
                }
            );
            if (!created)
            {
                timer.Change(delay, TimeSpan.FromMilliseconds(-1));
            }
        }

        private void PublishOnBackgroundThread<T>(T @event) where T : class
        {
            var notification = Wrap(@event);
            _eventAggregator.PublishOnBackgroundThread(notification);
            _logger.Debug("Published event {@Event} as notification on background thread.", @event);
        }

        private void PublishOnUIThread<T>(T @event) where T : class
        {
            var notification = Wrap(@event);
            _eventAggregator.PublishOnUIThread(notification);
            _logger.Debug("Published event {@Event} as notification on UI thread.", @event);
        }

        private static INotification Wrap<T>(T @event) where T : class
        {
            var notification = @event is INotification notificationEvent
                ? notificationEvent
                : new Notification<T>(@event);
            return notification;
        }
    }
}
