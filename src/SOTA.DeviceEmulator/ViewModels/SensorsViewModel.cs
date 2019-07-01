using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using EnsureThat;
using Serilog;
using SOTA.DeviceEmulator.Core;
using SOTA.DeviceEmulator.Core.Sensors;
using SOTA.DeviceEmulator.Core.Sensors.TimeFunctions;
using SOTA.DeviceEmulator.Services.Telemetry;

namespace SOTA.DeviceEmulator.ViewModels
{
    public sealed class SensorsViewModel : Screen, ITabViewModel, IHandle<TelemetryCollected>, ILocationSensorOptions, IPulseSensorOptions
    {
        private DeviceTelemetry _telemetry;
        private double _speedMean = 5;
        private double _speedDeviation = 1;
        private ITimeFunction<double> _pulseFunction;
        private int _pulseNoiseFactor = 3;

        public SensorsViewModel(ILogger logger, IEventAggregator eventAggregator, IEnumerable<ITimeFunction<double>> doubleTimeFunctions)
        {
            Ensure.Any.IsNotNull(eventAggregator, nameof(eventAggregator));

            TimeFunctions = doubleTimeFunctions.ToList();
            PulseFunction = TimeFunctions.First();

            eventAggregator.Subscribe(this);
            DisplayName = "Sensors";
            logger.Debug("Sensors view model created.");
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

        public ITimeFunction<double> PulseFunction
        {
            get => _pulseFunction;
            set => Set(ref _pulseFunction, value, nameof(PulseFunction));
        }

        public int PulseNoiseFactor
        {
            get => _pulseNoiseFactor;
            set => Set(ref _pulseNoiseFactor, value, nameof(PulseNoiseFactor));
        }

        public string LocationText => "Location";
        public string PulseText => "Pulse";
        public List<ITimeFunction<double>> TimeFunctions { get; }

        public void Handle(TelemetryCollected message)
        {
            Telemetry = message.Telemetry;
        }
    }
}