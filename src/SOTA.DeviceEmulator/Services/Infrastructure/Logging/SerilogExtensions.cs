using System;
using System.Linq;
using EnsureThat;
using FluentValidation.Results;
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

        public static void LogValidationErrorsIfAny(this ILogger logger, string messagePrefix, ValidationResult validationResult)
        {
            if (validationResult.IsValid)
            {
                return;
            }

            var errors = string.Join(
                Environment.NewLine,
                validationResult.Errors.Select(x => x.ErrorMessage).Select(x => $"- {x}")
            );

            logger.Warning($"{messagePrefix}:{Environment.NewLine}{errors}");
        }
    }
}