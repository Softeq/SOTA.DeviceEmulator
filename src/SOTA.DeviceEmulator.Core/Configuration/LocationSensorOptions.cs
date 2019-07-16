using EnsureThat;
using SOTA.DeviceEmulator.Core.Telemetry;

namespace SOTA.DeviceEmulator.Core.Configuration
{
    internal class LocationSensorOptions : ILocationSensorOptions
    {
        private readonly IDeviceConfigurationHolder _stateHolder;

        public LocationSensorOptions(IDeviceConfigurationHolder configurationHolder)
        {
            _stateHolder = Ensure.Any.IsNotNull(configurationHolder, nameof(configurationHolder));
        }

        public double SpeedMean
        {
            get => _stateHolder.Get(x => x.Location.SpeedMean);
            set => _stateHolder.Set(x => x.Location.SpeedMean, value);
        }

        public double SpeedDeviation
        {
            get => _stateHolder.Get(x => x.Location.SpeedDeviation);
            set => _stateHolder.Set(x => x.Location.SpeedDeviation, value);
        }
    }
}