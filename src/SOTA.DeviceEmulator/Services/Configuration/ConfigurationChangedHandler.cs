using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Serilog;
using SOTA.DeviceEmulator.Core;
using SOTA.DeviceEmulator.Core.Configuration;

namespace SOTA.DeviceEmulator.Services.Configuration
{
    internal class ConfigurationChangedHandler : INotificationHandler<Notification<DeviceConfigurationChanged>>
    {
        private readonly IDevice _device;
        private readonly IApplicationContext _applicationContext;
        private readonly IDeviceConfigurationSerializer _deviceConfigurationSerializer;
        private readonly ILogger _logger;

        public ConfigurationChangedHandler(
            ILogger logger,
            IApplicationContext applicationContext,
            IDeviceConfigurationSerializer deviceConfigurationSerializer,
            IDevice device
        )
        {
            _device = Ensure.Any.IsNotNull(device, nameof(device));
            _deviceConfigurationSerializer = Ensure.Any.IsNotNull(
                deviceConfigurationSerializer,
                nameof(deviceConfigurationSerializer)
            );
            _applicationContext = Ensure.Any.IsNotNull(applicationContext, nameof(applicationContext));
            _logger = logger.ForContext(GetType());
        }

        public async Task Handle(Notification<DeviceConfigurationChanged> notification, CancellationToken cancellationToken)
        {
            if (!_device.IsConnected)
            {
                return;
            }
            var isChanged = _device.TryGetChangedConfiguration(out var configuration);
            if (!isChanged)
            {
                return;
            }
            var reportedProperties =
                _deviceConfigurationSerializer.SerializeToDeviceProperties(configuration);
            await _applicationContext.DeviceClient.UpdateReportedPropertiesAsync(reportedProperties, cancellationToken);
            _logger.Information("Configuration change published: {@DeviceConfiguration}.", configuration);
        }
    }
}