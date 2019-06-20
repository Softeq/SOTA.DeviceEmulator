using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using EnsureThat;
using MediatR;
using SOTA.DeviceEmulator.Core;

namespace SOTA.DeviceEmulator.Services.Telemetry
{
    public class TelemetryCollectionHandler : INotificationHandler<TelemetryCollectionRequested>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IDevice _device;

        public TelemetryCollectionHandler(IEventAggregator eventAggregator, IDevice device)
        {
            _eventAggregator = Ensure.Any.IsNotNull(eventAggregator, nameof(eventAggregator));
            _device = Ensure.Any.IsNotNull(device, nameof(device));
        }

        public async Task Handle(TelemetryCollectionRequested notification, CancellationToken cancellationToken)
        {
            var telemetry = _device.ReportTelemetry();
            var @event = new TelemetryCollected(telemetry);
            await _eventAggregator.PublishOnUIThreadAsync(@event);
        }
    }
}
