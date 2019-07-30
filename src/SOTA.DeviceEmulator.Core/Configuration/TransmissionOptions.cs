using EnsureThat;

namespace SOTA.DeviceEmulator.Core.Configuration
{
    internal class TransmissionOptions : ITransmissionOptions
    {
        private readonly IDeviceConfigurationHolder _deviceStateHolder;

        public TransmissionOptions(IDeviceConfigurationHolder configurationHolder)
        {
            _deviceStateHolder = Ensure.Any.IsNotNull(configurationHolder, nameof(configurationHolder));
        }

        public bool Enabled
        {
            get => _deviceStateHolder.Get(x => x.Transmission.Enabled);
            set => _deviceStateHolder.Set(x => x.Transmission.Enabled, value);
        }

        public int Interval
        {
            get => _deviceStateHolder.Get(x => x.Transmission.Interval);
            set => _deviceStateHolder.Set(x => x.Transmission.Interval, value);
        }
    }
}