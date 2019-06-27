using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using EnsureThat;
using MediatR;
using Serilog;
using SOTA.DeviceEmulator.Core;

namespace SOTA.DeviceEmulator.Services.Telemetry
{
    public class TelemetryCollectionHandler : INotificationHandler<TelemetryCollectionRequested>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IDevice _device;
        private readonly ILogger _logger;

        public TelemetryCollectionHandler(IEventAggregator eventAggregator, IDevice device, ILogger logger)
        {
            _eventAggregator = Ensure.Any.IsNotNull(eventAggregator, nameof(eventAggregator));
            _device = Ensure.Any.IsNotNull(device, nameof(device));
            _logger = Ensure.Any.IsNotNull(logger, nameof(logger));
        }

        public async Task Handle(TelemetryCollectionRequested notification, CancellationToken cancellationToken)
        {
            var telemetryReport = _device.GetTelemetryReport();
            var @event = new TelemetryCollected(telemetryReport.Telemetry);
            await _eventAggregator.PublishOnUIThreadAsync(@event);

            if (telemetryReport.IsRequiredToSend)
            {
                _logger.Information("Reporting {@Telemetry}.", telemetryReport.Telemetry);
                // Sending to the iot hub will be here.
            }
        }
    }
}
