using EnsureThat;

namespace SOTA.DeviceEmulator.Services.Provisioning
{
    public class ConnectionModel
    {
        public ConnectionModel(string deviceDisplayName, bool isConnected)
        {
            IsConnected = isConnected;
            DeviceDisplayName = Ensure.String.IsNotNullOrEmpty(deviceDisplayName, nameof(deviceDisplayName));
        }

        public bool IsConnected { get; }

        public string DeviceDisplayName { get; }
    }
}