using System;
using Caliburn.Micro;

namespace SOTA.DeviceEmulator.ViewModels
{
    public sealed class ConnectionViewModel : Screen, ITabViewModel
    {
        public ConnectionViewModel()
        {
            DisplayName = "Connection";
            HeaderText = "Connection Settings";
        }

        public event EventHandler<ConnectionStatusEventArgs> ConnectionStatusChanged;

        public string HeaderText { get; }

        public string ConnectButtonText => _isConnected ? "Disconnect" : "Connect";

        private bool _isConnected;

        public void Connect()
        {
            _isConnected = !_isConnected;
            NotifyOfPropertyChange(() => ConnectButtonText);

            OnConnectionStatusChanged(new ConnectionStatusEventArgs { IsConnected = _isConnected });
        }

        private void OnConnectionStatusChanged(ConnectionStatusEventArgs e)
        {
            var eventHandler = ConnectionStatusChanged;
            eventHandler?.Invoke(this, e);
        }

        public class ConnectionStatusEventArgs : EventArgs
        {
            public bool IsConnected { get; set; }
        }
    }
}