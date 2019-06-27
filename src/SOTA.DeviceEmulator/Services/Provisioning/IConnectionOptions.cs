using System.Collections.Generic;

namespace SOTA.DeviceEmulator.Services.Provisioning
{
    public interface IConnectionOptions
    {
        IReadOnlyCollection<string> Environments { get; }

        string DefaultEnvironment { get; }

        string DeviceProvisioningServiceEndpoint { get; }

        string CertificatesFolderName { get; }

        string RootCertificateThumbprint { get; }

        string GetDeviceProvisioningServiceIdScope(string environment);
    }
}