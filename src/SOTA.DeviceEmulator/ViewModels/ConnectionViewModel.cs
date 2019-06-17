using System;
using Caliburn.Micro;

namespace SOTA.DeviceEmulator.ViewModels
{
    public sealed class ConnectionViewModel : Screen, ITabViewModel
    {
        private bool _isConnected;
        private string _connectButtonText = "Connect";

        public ConnectionViewModel()
        {
            DisplayName = "Connection";
            HeaderText = "Connection Settings";
        }

        public event EventHandler<ConnectionStatusEventArgs> ConnectionStatusChanged;

        public string HeaderText { get; }

        public string ConnectButtonText
        {
            get => _connectButtonText;
            set
            {
                _connectButtonText = value;
                NotifyOfPropertyChange(() => ConnectButtonText);
            }
        }

        public void Connect()
        {
            _isConnected = !_isConnected;
            ConnectButtonText = _isConnected ? "Disconnect" : "Connect";

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