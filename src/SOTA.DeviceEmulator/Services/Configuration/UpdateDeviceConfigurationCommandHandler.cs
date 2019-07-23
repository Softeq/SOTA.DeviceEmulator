using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Serilog;
using SOTA.DeviceEmulator.Core;
using SOTA.DeviceEmulator.Services.Infrastructure.Logging;

namespace SOTA.DeviceEmulator.Services.Configuration
{
    internal class UpdateDeviceConfigurationCommandHandler : IRequestHandler<UpdateDeviceConfigurationCommand>
    {
        private readonly IDevicePropertiesSerializer _devicePropertiesSerializer;
        private readonly IDevice _device;
        private readonly ILogger _logger;

        public UpdateDeviceConfigurationCommandHandler(
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

        public Task<Unit> Handle(UpdateDeviceConfigurationCommand request, CancellationToken cancellationToken)
        {
            Ensure.Any.IsNotNull(request.TwinCollection);

            var properties = _devicePropertiesSerializer.Deserialize(request.TwinCollection);
            var updateConfigValidationResult = _device.UpdateConfiguration(properties.Configuration);

            if (!updateConfigValidationResult.IsValid)
            {
                _logger.LogValidationErrors(updateConfigValidationResult);
            }
            else
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