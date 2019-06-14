using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Serilog;
using SOTA.DeviceEmulator.Services.Infrastructure.Jobs;

namespace SOTA.DeviceEmulator.Services.Telemetry
{
    public class TelemetryReportingHandler : INotificationHandler<TelemetryCollected>, IToggleable
    {
        private readonly ILogger _logger;

        public TelemetryReportingHandler(ILogger logger)
        {
            _logger = Ensure.Any.IsNotNull(logger, nameof(logger));
        }

        public Task Handle(TelemetryCollected notification, CancellationToken cancellationToken)
        {
            _logger.Information("Reporting {@Telemetry}.", notification);
            return Task.CompletedTask;
        }

        public bool IsEnabled => true;
    }
}
