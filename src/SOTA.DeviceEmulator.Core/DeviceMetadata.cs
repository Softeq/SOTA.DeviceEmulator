using EnsureThat;
using SOTA.DeviceEmulator.Core.Configuration;

namespace SOTA.DeviceEmulator.Core
{
    public class DeviceMetadata
    {
        public DeviceMetadata(
            string deviceId,
            string registrationId,
            DeviceConfiguration reportedConfiguration,
            DeviceConfiguration desiredConfiguration
        )
        {
            ReportedConfiguration = reportedConfiguration;
            DesiredConfiguration = desiredConfiguration;
            DeviceId = Ensure.String.IsNotNullOrEmpty(deviceId, nameof(deviceId));
            RegistrationId = Ensure.String.IsNotNullOrEmpty(registrationId, nameof(registrationId));
        }

        public string DeviceId { get; }

        public string RegistrationId { get; }

        public DeviceConfiguration ReportedConfiguration { get; }

        public DeviceConfiguration DesiredConfiguration { get; }
    }
}