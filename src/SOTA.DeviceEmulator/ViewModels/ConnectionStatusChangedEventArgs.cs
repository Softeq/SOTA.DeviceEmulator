using System;

namespace SOTA.DeviceEmulator.ViewModels
{
    public class ConnectionStatusChangedEventArgs : EventArgs
    {
        public ConnectionStatusChangedEventArgs(bool isConnected, string deviceDisplayName)
        {
            IsConnected = isConnected;
            DeviceDisplayName = deviceDisplayName;
        }

        public bool IsConnected { get; }

        public string DeviceDisplayName { get; }
    }
}
