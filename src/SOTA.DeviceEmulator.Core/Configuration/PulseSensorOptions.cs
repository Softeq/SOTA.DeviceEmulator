using System;
using System.Collections.Generic;
using System.Linq;
using EnsureThat;
using SOTA.DeviceEmulator.Core.Telemetry;
using SOTA.DeviceEmulator.Core.Telemetry.TimeFunctions;

namespace SOTA.DeviceEmulator.Core.Configuration
{
    internal class PulseSensorOptions : IPulseSensorOptions
    {
        private readonly Dictionary<string, ITimeFunction<double>> _doubleTimeFunctions;
        private readonly IDeviceConfigurationHolder _stateHolder;

        public PulseSensorOptions(IDeviceConfigurationHolder configurationHolder, IEnumerable<ITimeFunction<double>> doubleTimeFunctions)
        {
            Ensure.Any.IsNotNull(doubleTimeFunctions, nameof(doubleTimeFunctions));
            _stateHolder = Ensure.Any.IsNotNull(configurationHolder, nameof(configurationHolder));

            _doubleTimeFunctions = doubleTimeFunctions.ToDictionary(x => x.DisplayName);
        }

        public int NoiseFactor
        {
            get => _stateHolder.Get(x => x.Pulse.NoiseFactor);
            set => _stateHolder.Set(x => x.Pulse.NoiseFactor, value);
        }

        public ITimeFunction<double> Function
        {
            get
            {
                var name = _stateHolder.Get(x => x.Pulse.Algorithm);
                if (!_doubleTimeFunctions.ContainsKey(name))
                {
                    throw new InvalidOperationException($"Unknown pulse function: {name}.");
                }
                return _doubleTimeFunctions[name];
            }
            set
            {
                Ensure.Any.IsNotNull(value, nameof(value));

                _stateHolder.Set(x => x.Pulse.Algorithm, value.DisplayName);
            }
        }
    }
}