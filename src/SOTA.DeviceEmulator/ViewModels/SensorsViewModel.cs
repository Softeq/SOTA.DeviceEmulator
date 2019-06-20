using Caliburn.Micro;
using EnsureThat;
using Serilog;
using SOTA.DeviceEmulator.Core;
using SOTA.DeviceEmulator.Core.Sensors.TimeFunctions;
using SOTA.DeviceEmulator.Services.Telemetry;

namespace SOTA.DeviceEmulator.ViewModels
{
    public sealed class SensorsViewModel : Screen, ITabViewModel, IHandle<TelemetryCollected>
    {
        private DeviceTelemetry _telemetry;
        private double _speedMean = 2.5;
        private double _speedDeviation = 1.5;
        private FunctionType _pulseAlgorithm = FunctionType.Harmonic;
        private int _pulseNoiseFactor = 3;

        private readonly IDevice _device;

        public SensorsViewModel(ILogger logger, IEventAggregator eventAggregator, IDevice device)
        {
            Ensure.Any.IsNotNull(eventAggregator, nameof(eventAggregator));
            _device = Ensure.Any.IsNotNull(device, nameof(device));

            ConfigureDeviceSensors();

            eventAggregator.Subscribe(this);
            DisplayName = "Sensors";
            logger.Debug("Sensors view model created.");

            PropertyChanged += SensorsViewModel_PropertyChanged;
        }

        public DeviceTelemetry Telemetry
        {
            get => _telemetry;
            set => Set(ref _telemetry, value, nameof(Telemetry));
        }

        public double SpeedMean
        {
            get => _speedMean;
            set => Set(ref _speedMean, value, nameof(SpeedMean));
        }

        public double SpeedDeviation
        {
            get => _speedDeviation;
            set => Set(ref _speedDeviation, value, nameof(SpeedDeviation));
        }

        public FunctionType PulseAlgorithm
        {
            get => _pulseAlgorithm;
            set => Set(ref _pulseAlgorithm, value, nameof(PulseAlgorithm));
        }

        public int PulseNoiseFactor
        {
            get => _pulseNoiseFactor;
            set => Set(ref _pulseNoiseFactor, value, nameof(PulseNoiseFactor));
        }

        public string LocationText => "Location";
        public string PulseText => "Pulse";

        public void Handle(TelemetryCollected message)
        {
            Telemetry = message.Value;
        }

        private void SensorsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SpeedMean):
                    SetSpeedMean();
                    break;
                case nameof(SpeedDeviation):
                    SetSpeedDeviation();
                    break;
                case nameof(PulseAlgorithm):
                    SetPulseAlgorithm();
                    break;
                case nameof(PulseNoiseFactor):
                    SetPulseNoiseFactor();
                    break;
            }
        }

        private void ConfigureDeviceSensors()
        {
            SetSpeedMean();
            SetSpeedDeviation();
            SetPulseAlgorithm();
            SetPulseNoiseFactor();
        }

        private void SetSpeedMean()
        {
            // UI has speed in m/s so need to convert to km/h
            var speedInKilometers = SpeedMean * 3.6;
            _device.LocationSensor.SpeedMean = speedInKilometers;
        }

        private void SetSpeedDeviation()
        {
            // UI has speed in m/s so need to convert to km/h
            var speedInKilometers = SpeedDeviation * 3.6;
            _device.LocationSensor.SpeedDeviation = speedInKilometers;
        }

        private void SetPulseAlgorithm()
        {
            switch (PulseAlgorithm)
            {
                case FunctionType.Harmonic:
                    _device.PulseSensor.Function = new PulseHarmonicFunction();
                    break;
            }
        }

        private void SetPulseNoiseFactor()
        {
            _device.PulseSensor.NoiseFactor = _pulseNoiseFactor;
        }
    }
}