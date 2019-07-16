using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Serilog;
using SOTA.DeviceEmulator.Services.Infrastructure.Serialization;

namespace SOTA.DeviceEmulator.Services.Telemetry
{
    internal class TelemetryPublishedHandler : INotificationHandler<TelemetryPublished>
    {
        private readonly IApplicationContext _applicationContext;
        private readonly IIoTHubMessageSerializer _iotHubMessageSerializer;
        private readonly ILogger _logger;

        public TelemetryPublishedHandler(
            IIoTHubMessageSerializer iotHubMessageSerializer,
            IApplicationContext applicationContext,
            ILogger logger
        )
        {
            _iotHubMessageSerializer = Ensure.Any.IsNotNull(iotHubMessageSerializer, nameof(iotHubMessageSerializer));
            _applicationContext = Ensure.Any.IsNotNull(applicationContext, nameof(applicationContext));
            _logger = Ensure.Any.IsNotNull(logger, nameof(logger));
        }

        public async Task Handle(TelemetryPublished notification, CancellationToken cancellationToken)
        {
            var @event = _iotHubMessageSerializer.Serialize(notification.Telemetry);
            await _applicationContext.DeviceClient.SendEventAsync(@event, cancellationToken);
            _logger.Information("Telemetry is published: {@DeviceTelemetry}.", notification.Telemetry);
        }
    }
}