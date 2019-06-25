using EnsureThat;
using Serilog;
using Serilog.Events;

namespace SOTA.DeviceEmulator.Services.Infrastructure.Logging
{
    internal static class SerilogExtensions
    {
        public static LoggerConfiguration UseSotaDeviceEmulatorConfiguration(
            this LoggerConfiguration configuration,
            ObservableCollectionLogEventSink sink,
            LogEventLevel minimumLogLevel
        )
        {
            Ensure.Any.IsNotNull(configuration, nameof(configuration));

            return configuration
                   .MinimumLevel.Is(minimumLogLevel)
                   .WriteTo.Sink(sink);
        }
    }
}