using Caliburn.Micro;

namespace SOTA.DeviceEmulator.ViewModels
{
    public class StatusBarViewModel : PropertyChangedBase
    {
        private string _connectionStatus = "Offline";

        public string ConnectionStatus
        {
            get => _connectionStatus;
            set
            {
                _connectionStatus = value;
                NotifyOfPropertyChange(() => ConnectionStatus);
            }
        }

        public void ConnectionStatusChanged(object sender, ConnectionViewModel.ConnectionStatusEventArgs e)
        {
            ConnectionStatus = e.IsConnected ? "Online" : "Offline";
        }
    }
}