using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using EnsureThat;
using MediatR;
using Newtonsoft.Json;
using Serilog;
using SOTA.DeviceEmulator.Core;
using Message = Microsoft.Azure.Devices.Client.Message;

namespace SOTA.DeviceEmulator.Services.Telemetry
{
    public class TelemetryCollectionHandler : INotificationHandler<TelemetryCollectionRequested>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IDevice _device;
        private readonly IApplicationContext _applicationContext;
        private readonly ILogger _logger;
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public TelemetryCollectionHandler(
            IEventAggregator eventAggregator,
            IDevice device,
            IApplicationContext applicationContext,
            ILogger logger,
            JsonSerializerSettings jsonSerializerSettings)
        {
            _eventAggregator = Ensure.Any.IsNotNull(eventAggregator, nameof(eventAggregator));
            _device = Ensure.Any.IsNotNull(device, nameof(device));
            _applicationContext = Ensure.Any.IsNotNull(applicationContext, nameof(applicationContext));
            _logger = Ensure.Any.IsNotNull(logger, nameof(logger));
            _jsonSerializerSettings = Ensure.Any.IsNotNull(jsonSerializerSettings, nameof(jsonSerializerSettings));
        }

        public async Task Handle(TelemetryCollectionRequested notification, CancellationToken cancellationToken)
        {
            var telemetryReport = _device.GetTelemetryReport();
            var @event = new TelemetryCollected(telemetryReport.Telemetry, _device.SessionTime);
            await _eventAggregator.PublishOnUIThreadAsync(@event);

            if (telemetryReport.IsPublished)
            {
                _logger.Information("Reporting {@Telemetry}.", telemetryReport.Telemetry);

                var jsonPayload = JsonConvert.SerializeObject(telemetryReport.Telemetry, _jsonSerializerSettings);
                var message = new Message(Encoding.ASCII.GetBytes(jsonPayload));

                await _applicationContext.DeviceClient.SendEventAsync(message, cancellationToken);
            }
        }
    }
}
