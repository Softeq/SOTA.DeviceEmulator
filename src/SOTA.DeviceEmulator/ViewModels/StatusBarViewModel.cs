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
    }
}