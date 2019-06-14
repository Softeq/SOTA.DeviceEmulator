using System;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using EnsureThat;
using MediatR;

namespace SOTA.DeviceEmulator.Services.Telemetry
{
    public class TelemetryCollectionHandler : INotificationHandler<TelemetryCollectionRequested>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly Random _random;

        public TelemetryCollectionHandler(IEventAggregator eventAggregator)
        {
            _eventAggregator = Ensure.Any.IsNotNull(eventAggregator, nameof(eventAggregator));
            _random = new Random();
        }

        public async Task Handle(TelemetryCollectionRequested notification, CancellationToken cancellationToken)
        {
            var value = _random.Next(1, 100);
            var @event = new TelemetryCollected(value);
            await _eventAggregator.PublishOnUIThreadAsync(@event);
        }
    }
}
