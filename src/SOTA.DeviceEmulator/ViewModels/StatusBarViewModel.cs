using Caliburn.Micro;

namespace SOTA.DeviceEmulator.ViewModels
{
    public class StatusBarViewModel : PropertyChangedBase
    {
        private bool _isConnected;
        private bool _isTransmissionEnabled;
        private int _transmissionPeriod = 3;

        public bool IsConnected
        {
            get => _isConnected;
            set => Set(ref _isConnected, value, nameof(IsConnected));
        }

        public bool IsTransmissionEnabled
        {
            get => _isTransmissionEnabled;
            set => Set(ref _isTransmissionEnabled, value, nameof(IsTransmissionEnabled));
        }

        public int TransmissionPeriod
        {
            get => _transmissionPeriod;
            set => Set(ref _transmissionPeriod, value, nameof(TransmissionPeriod));
        }
    }
}