using EnsureThat;

namespace SOTA.DeviceEmulator.Core
{
    public class DeviceConnectionMetadata
    {
        public DeviceConnectionMetadata(string deviceId, string registrationId)
        {
            DeviceId = Ensure.String.IsNotNullOrEmpty(deviceId, nameof(deviceId));
            RegistrationId = Ensure.String.IsNotNullOrEmpty(registrationId, nameof(registrationId));
        }

        public string DeviceId { get; }

        public string RegistrationId { get; }
    }
}
