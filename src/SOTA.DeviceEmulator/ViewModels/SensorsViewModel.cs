using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using EnsureThat;
using SOTA.DeviceEmulator.Core.Configuration;
using SOTA.DeviceEmulator.Core.Telemetry;
using SOTA.DeviceEmulator.Core.Telemetry.TimeFunctions;
using SOTA.DeviceEmulator.Services;
using SOTA.DeviceEmulator.Services.Telemetry;

namespace SOTA.DeviceEmulator.ViewModels
{
    public sealed class SensorsViewModel : Screen, ITabViewModel, IHandle<TelemetryCollected>, IHandle<Notification<DeviceConfigurationDownloaded>>
    {
        private readonly IDeviceConfiguration _deviceState;
        private DeviceTelemetry _telemetry;

        public SensorsViewModel(
            IDeviceConfiguration deviceState,
            IEventAggregator eventAggregator,
            IEnumerable<ITimeFunction<double>> doubleTimeFunctions
        )
        {
            _deviceState = Ensure.Any.IsNotNull(deviceState, nameof(deviceState));
            Ensure.Any.IsNotNull(eventAggregator, nameof(eventAggregator));

            TimeFunctions = doubleTimeFunctions.ToList();
            eventAggregator.Subscribe(this);
            DisplayName = "Sensors";
        }

        public DeviceTelemetry Telemetry
        {
            get => _telemetry;
            set => Set(ref _telemetry, value, nameof(Telemetry));
        }

        public double SpeedMean
        {
            get => _deviceState.Location.SpeedMean;
            set
            {
                _deviceState.Location.SpeedMean = value;
                NotifyOfPropertyChange(nameof(SpeedMean));
            }
        }

        public double SpeedDeviation
        {
            get => _deviceState.Location.SpeedDeviation;
            set
            {
                _deviceState.Location.SpeedDeviation = value;
                NotifyOfPropertyChange(nameof(SpeedDeviation));
            }
        }

        public ITimeFunction<double> PulseFunction
        {
            get => _deviceState.Pulse.Function;
            set
            {
                _deviceState.Pulse.Function = value;
                NotifyOfPropertyChange(nameof(PulseFunction));
            }
        }

        public int NoiseFactor
        {
            get => _deviceState.Pulse.NoiseFactor;
            set
            {
                _deviceState.Pulse.NoiseFactor = value;
                NotifyOfPropertyChange(nameof(NoiseFactor));
            }
        }

        public string LocationText => "Location";
        public string PulseText => "Pulse";
        public List<ITimeFunction<double>> TimeFunctions { get; }

        public void Handle(TelemetryCollected message)
        {
            Telemetry = message.Telemetry;
        }

        public void Handle(Notification<DeviceConfigurationDownloaded> message)
        {
            NotifyOfPropertyChange(null);
        }
    }
}