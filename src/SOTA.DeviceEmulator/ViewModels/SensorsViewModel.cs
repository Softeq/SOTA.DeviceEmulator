using Caliburn.Micro;
using EnsureThat;
using Serilog;
using SOTA.DeviceEmulator.Services.Telemetry;

namespace SOTA.DeviceEmulator.ViewModels
{
    public sealed class SensorsViewModel : Screen, ITabViewModel, IHandle<TelemetryCollected>
    {
        private int? _value;

        public SensorsViewModel(ILogger logger, IEventAggregator eventAggregator)
        {
            Ensure.Any.IsNotNull(eventAggregator, nameof(eventAggregator));
            eventAggregator.Subscribe(this);
            DisplayName = "Sensors";
            logger.Debug("Sensors view model created.");
        }

        public int? Value
        {
            get => _value;
            set => Set(ref _value, value, nameof(Value));
        }

        public void Handle(TelemetryCollected message)
        {
            Value = message.Value;
        }
    }
}