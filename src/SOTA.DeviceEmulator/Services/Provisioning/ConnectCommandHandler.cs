using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Provisioning.Client;
using Microsoft.Azure.Devices.Provisioning.Client.Transport;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using Serilog;
using SOTA.DeviceEmulator.Core;

namespace SOTA.DeviceEmulator.Services.Provisioning
{
    public class ConnectCommandHandler : IRequestHandler<ConnectCommand, ConnectionModel>
    {
        private const string CertificatePassword = "sota";
        private readonly IApplicationContext _applicationContext;
        private readonly IDevice _device;
        private readonly JsonSerializerSettings _jsonSerializerSettings;
        private readonly ILogger _logger;
        private readonly IMediator _mediator;

        public ConnectCommandHandler(
            IDevice device,
            JsonSerializerSettings jsonSerializerSettings,
            ILogger logger,
            IMediator mediator,
            IApplicationContext applicationContext
        )
        {
            _mediator = Ensure.Any.IsNotNull(mediator, nameof(mediator));
            _applicationContext = Ensure.Any.IsNotNull(applicationContext, nameof(applicationContext));
            _logger = Ensure.Any.IsNotNull(logger, nameof(logger));
            _device = Ensure.Any.IsNotNull(device, nameof(device));
            _jsonSerializerSettings = Ensure.Any.IsNotNull(jsonSerializerSettings, nameof(jsonSerializerSettings));
        }

        public async Task<ConnectionModel> Handle(ConnectCommand request, CancellationToken cancellationToken)
        {
            var disposables = new List<IDisposable>();

            DeviceClient deviceClient = null;
            try
            {
                var certificates = new X509Certificate2Collection();
                certificates.Import(request.CertificatePath, CertificatePassword, X509KeyStorageFlags.UserKeySet);
                var certificate = certificates.Cast<X509Certificate2>().FirstOrDefault(x => x.HasPrivateKey);
                disposables.AddRange(certificates.Cast<X509Certificate2>().Where(c => !c.Equals(certificate)));
                if (certificate == null)
                {
                    throw new InvalidOperationException("Failed to find a valid certificate.");
                }

                using (var security = new SecurityProviderX509Certificate(certificate))
                using (var transport = new ProvisioningTransportHandlerAmqp(TransportFallbackType.TcpOnly))
                {
                    var provisioningClient = ProvisioningDeviceClient.Create(
                        request.DeviceProvisioningServiceEndpoint,
                        request.DeviceProvisioningServiceIdScope,
                        security,
                        transport
                    );
                    var metadataJson = JsonConvert.SerializeObject(_device.Metadata, _jsonSerializerSettings);
                    var registration = new ProvisioningRegistrationAdditionalData {JsonData = metadataJson};
                    var result = await provisioningClient.RegisterAsync(registration, cancellationToken);
                    _logger.Information(
                        "Device is registered. Device ID: {DeviceId}. Registration ID: {RegistrationId}.",
                        result.DeviceId,
                        result.RegistrationId
                    );
                    var deviceAuth = new DeviceAuthenticationWithX509Certificate(
                        result.DeviceId,
                        security.GetAuthenticationCertificate()
                    );
                    deviceClient = DeviceClient.Create(result.AssignedHub, deviceAuth, TransportType.Amqp);
                    await deviceClient.OpenAsync(cancellationToken);
                    await _mediator.Send(new DisconnectCommand(), cancellationToken);
                    _applicationContext.DeviceClient = deviceClient;
                    var connectionMetadata = new DeviceConnectionMetadata(result.DeviceId, result.RegistrationId);
                    _device.Connect(connectionMetadata);
                    _logger.Information("Device is connected.");
                    return new ConnectionModel(_device.DisplayName, _device.IsConnected);
                }
            }
            catch
            {
                if (deviceClient != null)
                {
                    disposables.Add(deviceClient);
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
    }
}