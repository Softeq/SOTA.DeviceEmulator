using System;

namespace SOTA.DeviceEmulator.ViewModels
{
    public class ConnectionStatusChangedEventArgs : EventArgs
    {
        public bool IsConnected { get; set; }
    }
}
