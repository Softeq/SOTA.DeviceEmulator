using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using EnsureThat;
using MediatR;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Provisioning.Client;
using Microsoft.Azure.Devices.Provisioning.Client.Transport;
using Microsoft.Azure.Devices.Shared;
using Serilog;
using SOTA.DeviceEmulator.Core;
using SOTA.DeviceEmulator.Services.Configuration;
using SOTA.DeviceEmulator.Services.Infrastructure.Logging;

namespace SOTA.DeviceEmulator.Services.Provisioning
{
    internal class ConnectCommandHandler : IRequestHandler<ConnectCommand, ConnectionModel>
    {
        private readonly IDevicePropertiesSerializer _devicePropertiesSerializer;
        private const string CertificatePassword = "sota";
        private readonly IApplicationContext _applicationContext;
        private readonly IDevice _device;
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        private readonly IEventPublisher _eventPublisher;

        public ConnectCommandHandler(
            IDevice device,
            IDevicePropertiesSerializer devicePropertiesSerializer,
            ILogger logger,
            IMediator mediator,
            IApplicationContext applicationContext,
            IEventPublisher eventPublisher
        )
        {
            _devicePropertiesSerializer = Ensure.Any.IsNotNull(
                devicePropertiesSerializer,
                nameof(devicePropertiesSerializer)
            );
            _mediator = Ensure.Any.IsNotNull(mediator, nameof(mediator));
            _applicationContext = Ensure.Any.IsNotNull(applicationContext, nameof(applicationContext));
            _logger = Ensure.Any.IsNotNull(logger, nameof(logger)).ForContext(GetType());
            _device = Ensure.Any.IsNotNull(device, nameof(device));
            _eventPublisher = Ensure.Any.IsNotNull(eventPublisher, nameof(eventPublisher));
        }

        public async Task<ConnectionModel> Handle(ConnectCommand request, CancellationToken cancellationToken)
        {
            var disposables = new List<IDisposable>();

            DeviceClient deviceClient = null;
            try
            {
                var certificateChain = new X509Certificate2Collection();
                certificateChain.Import(request.CertificatePath, CertificatePassword, X509KeyStorageFlags.UserKeySet);
                var deviceCertificate = certificateChain.Cast<X509Certificate2>().FirstOrDefault(x => x.HasPrivateKey);
                if (deviceCertificate == null)
                {
                    throw new InvalidOperationException("Failed to find a valid device certificate.");
                }
                certificateChain.Remove(deviceCertificate);
                using (var security = new SecurityProviderX509Certificate(deviceCertificate, certificateChain))
                using (var transport = new ProvisioningTransportHandlerAmqp(TransportFallbackType.TcpOnly))
                {
                    var provisioningClient = ProvisioningDeviceClient.Create(
                        request.DeviceProvisioningServiceEndpoint,
                        request.DeviceProvisioningServiceIdScope,
                        security,
                        transport
                    );
                    var registration = _devicePropertiesSerializer.SerializeToRegistrationData(_device.Information);
                    var registrationResult = await provisioningClient.RegisterAsync(registration, cancellationToken);
                    _logger.Information(
                        "Device is registered. Device ID: {DeviceId}. Registration ID: {RegistrationId}.",
                        registrationResult.DeviceId,
                        registrationResult.RegistrationId
                    );
                    var deviceAuth = new DeviceAuthenticationWithX509Certificate(
                        registrationResult.DeviceId,
                        security.GetAuthenticationCertificate()
                    );
                    deviceClient = DeviceClient.Create(registrationResult.AssignedHub, deviceAuth, TransportType.Amqp);
                    await deviceClient.OpenAsync(cancellationToken);
                    _logger.Information("Device client connection is open.");
                    await _mediator.Send(new DisconnectCommand(), cancellationToken);
                    _applicationContext.DeviceClient = deviceClient;

                    await deviceClient.SetDesiredPropertyUpdateCallbackAsync(OnDesiredPropertyChange, null, cancellationToken);

                    var twin = await deviceClient.GetTwinAsync(cancellationToken);
                    _logger.Debug("Initial device twin received: {@DeviceTwin}.", twin);
                    var reportedProperties = _devicePropertiesSerializer.Deserialize(twin.Properties.Reported);
                    var desiredProperties = _devicePropertiesSerializer.Deserialize(twin.Properties.Desired);
                    
                    var metadata = new DeviceMetadata(
                        registrationResult.DeviceId,
                        registrationResult.RegistrationId,
                        reportedProperties.Configuration,
                        desiredProperties.Configuration
                    );
                    var validationResult = _device.Connect(metadata);
                    _logger.Information("Device is connected.");
                    _device.TryGetChangedConfiguration(out var actualConfiguration);
                    var actualProperties = new DeviceProperties(_device.Information, actualConfiguration);
                    if (!EqualityComparer<DeviceProperties>.Default.Equals(
                        reportedProperties,
                        actualProperties
                    ))
                    {
                        var twinProperties =
                            _devicePropertiesSerializer.Serialize(actualProperties);
                        await deviceClient.UpdateReportedPropertiesAsync(twinProperties, cancellationToken);
                        _logger.Information(
                            "Device properties were reported: {@DeviceProperties}.",
                            actualProperties
                        );
                    }

                    _logger.LogValidationErrorsIfAny("Invalid device configuration provided", validationResult);

                    return new ConnectionModel(_device.DisplayName, _device.IsConnected);
                }
            }
            catch
            {
                if (deviceClient != null)
                {
                    disposables.Add(deviceClient);
                    _applicationContext.DeviceClient = null;
                }

                throw;
            }
            finally
            {
                foreach (var disposable in disposables)
                {
                    try
                    {
                        disposable.Dispose();
                    }
                    catch
                    {
                        // Do nothing
                    }
                }
            }
        }

        private Task OnDesiredPropertyChange(TwinCollection twinCollection, object userContext)
        {
            var updateConfigurationEvent = new DeviceConfigurationRemotelyUpdated
            {
                TwinCollection = twinCollection
            };

            _eventPublisher.Publish(updateConfigurationEvent);

            return Task.CompletedTask;
        }
    }
}