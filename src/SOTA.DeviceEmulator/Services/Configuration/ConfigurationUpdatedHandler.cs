using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Serilog;
using SOTA.DeviceEmulator.Core;
using SOTA.DeviceEmulator.Services.Infrastructure.Logging;

namespace SOTA.DeviceEmulator.Services.Configuration
{
    internal class ConfigurationUpdatedHandler : INotificationHandler<Notification<DeviceConfigurationUpdated>>
    {
        private readonly IDevicePropertiesSerializer _devicePropertiesSerializer;
        private readonly IDevice _device;
        private readonly ILogger _logger;

        public ConfigurationUpdatedHandler(
            IDevicePropertiesSerializer devicePropertiesSerializer,
            IDevice device,
            ILogger logger
        )
        {
            _devicePropertiesSerializer = Ensure.Any.IsNotNull(
                devicePropertiesSerializer,
                nameof(devicePropertiesSerializer)
            );

            _device = Ensure.Any.IsNotNull(device, nameof(device));
            _logger = Ensure.Any.IsNotNull(logger, nameof(logger)).ForContext(GetType());
        }

        public Task<Unit> Handle(DeviceConfigurationUpdated request, CancellationToken cancellationToken)
        {
            Ensure.Any.IsNotNull(request.TwinCollection);

            var properties = _devicePropertiesSerializer.Deserialize(request.TwinCollection);
            var updateConfigValidationResult = _device.UpdateConfiguration(properties.Configuration);

            _logger.LogValidationErrorsIfAny("Invalid device configuration provided", updateConfigValidationResult);

            if (updateConfigValidationResult.IsValid)
            {
                _logger.Information(
                    "Device properties were updated: {@DeviceProperties}.",
                    properties
                );
            }

            return Task.FromResult(Unit.Value);
        }

        public Task Handle(Notification<DeviceConfigurationUpdated> notification, CancellationToken cancellationToken)
        {
            Ensure.Any.IsNotNull(notification.Event.TwinCollection);

            var properties = _devicePropertiesSerializer.Deserialize(notification.Event.TwinCollection);
            var updateConfigValidationResult = _device.UpdateConfiguration(properties.Configuration);

            _logger.LogValidationErrorsIfAny("Invalid device configuration provided", updateConfigValidationResult);

            if (updateConfigValidationResult.IsValid)
            {
                _logger.Information(
                    "Device properties were updated: {@DeviceProperties}.",
                    properties
                );
            }

            return Task.FromResult(Unit.Value);
        }
    }
}