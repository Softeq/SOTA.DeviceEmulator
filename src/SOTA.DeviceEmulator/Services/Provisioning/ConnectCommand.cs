using EnsureThat;
using MediatR;

namespace SOTA.DeviceEmulator.Services.Provisioning
{
    public class ConnectCommand : IRequest<ConnectionModel>
    {
        public ConnectCommand(
            string certificatePath,
            string deviceProvisioningServiceEndpoint,
            string deviceProvisioningServiceIdScope
        )
        {
            CertificatePath = Ensure.String.IsNotNullOrEmpty(certificatePath, nameof(certificatePath));
            DeviceProvisioningServiceEndpoint = Ensure.String.IsNotNullOrEmpty(
                deviceProvisioningServiceEndpoint,
                nameof(deviceProvisioningServiceEndpoint)
            );
            DeviceProvisioningServiceIdScope = Ensure.String.IsNotNullOrEmpty(
                deviceProvisioningServiceIdScope,
                nameof(deviceProvisioningServiceIdScope)
            );
        }

        public string CertificatePath { get; }

        public string DeviceProvisioningServiceEndpoint { get; }

        public string DeviceProvisioningServiceIdScope { get; }
    }
}