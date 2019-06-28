using System;
using Caliburn.Micro;
using EnsureThat;
using SOTA.DeviceEmulator.Core;
using SOTA.DeviceEmulator.Services.Telemetry;

namespace SOTA.DeviceEmulator.ViewModels
{
    public class StatusBarViewModel : PropertyChangedBase, IHandle<TelemetryCollected>, ITransmissionOptions
    {
        private bool _isConnected;
        private bool _isTransmissionEnabled;
        private TimeSpan _sessionTime;

        public StatusBarViewModel(IEventAggregator eventAggregator)
        {
            Ensure.Any.IsNotNull(eventAggregator, nameof(eventAggregator));

            eventAggregator.Subscribe(this);
        }

        // Default period in seconds.
        private int _transmissionPeriod = 3;

        public bool IsConnected
        {
            get => _isConnected;
            set => Set(ref _isConnected, value, nameof(IsConnected));
        }

        public TimeSpan SessionTime
        {
            get => _sessionTime;
            set => Set(ref _sessionTime, value, nameof(SessionTime));
        }

        public bool Enabled
        {
            get => _isTransmissionEnabled;
            set => Set(ref _isTransmissionEnabled, value, nameof(Enabled));
        }

        public int Interval
        {
            get => _transmissionPeriod;
            set => Set(ref _transmissionPeriod, value, nameof(Interval));
        }

        public void Handle(TelemetryCollected message)
        {
            SessionTime = message.SessionTime;
        }
    }
}