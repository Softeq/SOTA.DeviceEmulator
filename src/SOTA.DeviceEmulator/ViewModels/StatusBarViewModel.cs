using System;
using Caliburn.Micro;
using EnsureThat;
using SOTA.DeviceEmulator.Core.Configuration;
using SOTA.DeviceEmulator.Services.Telemetry;

namespace SOTA.DeviceEmulator.ViewModels
{
    public class StatusBarViewModel : PropertyChangedBase, IHandle<TelemetryCollected>
    {
        private TimeSpan _sessionTime;
        private readonly IDeviceConfiguration _deviceConfiguration;
        private bool _isConnected;

        public StatusBarViewModel(IEventAggregator eventAggregator, IDeviceConfiguration deviceConfiguration)
        {
            _deviceConfiguration = Ensure.Any.IsNotNull(deviceConfiguration, nameof(deviceConfiguration));
            Ensure.Any.IsNotNull(eventAggregator, nameof(eventAggregator));

            eventAggregator.Subscribe(this);
        }

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
            get => _deviceConfiguration.Transmission.Enabled;
            set
            {
                _deviceConfiguration.Transmission.Enabled = value;
                NotifyOfPropertyChange(nameof(Enabled));
            }
        }

        public int Interval
        {
            get => _deviceConfiguration.Transmission.Interval;
            set
            {
                _deviceConfiguration.Transmission.Interval = value;
                NotifyOfPropertyChange(nameof(Interval));
            }
        }

        public void Handle(TelemetryCollected message)
        {
            SessionTime = message.SessionTime;
        }
    }
}