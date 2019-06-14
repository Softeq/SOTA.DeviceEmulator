using System.Collections.ObjectModel;
using EnsureThat;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;

namespace SOTA.DeviceEmulator.Services.Infrastructure.Logging
{
    internal static class SerilogExtensions
    {
        public static LoggerConfiguration UseSotaDeviceEmulatorConfiguration(
            this LoggerConfiguration configuration,
            ObservableCollection<LogEventViewModel> logEventCollection,
            LogEventLevel minimumLogLevel)
        {
            Ensure.Any.IsNotNull(logEventCollection, nameof(logEventCollection));
            Ensure.Any.IsNotNull(configuration, nameof(configuration));

            return configuration
                .MinimumLevel.Is(minimumLogLevel)
                .WriteTo.ObservableCollection(logEventCollection);
        }

        public static LoggerConfiguration ObservableCollection(this LoggerSinkConfiguration loggerSinkConfiguration,
            ObservableCollection<LogEventViewModel> logEventCollection)
        {
            return loggerSinkConfiguration.Sink(new ObservableCollectionLogEventSink(logEventCollection));
        }
    }
}