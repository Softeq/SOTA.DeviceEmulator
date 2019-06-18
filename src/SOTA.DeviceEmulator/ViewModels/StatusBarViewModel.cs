using Caliburn.Micro;

namespace SOTA.DeviceEmulator.ViewModels
{
    public class StatusBarViewModel : PropertyChangedBase
    {
        private bool _isConnected;

        public bool IsConnected
        {
            get => _isConnected;
            set => Set(ref _isConnected, value, nameof(IsConnected));
        }

        public void ToggleConnectionStatus(object sender, ConnectionStatusChangedEventArgs @event)
        {
            IsConnected = @event.IsConnected;
        }
    }
}