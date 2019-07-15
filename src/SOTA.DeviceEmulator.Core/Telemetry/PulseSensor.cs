using System;
using EnsureThat;

namespace SOTA.DeviceEmulator.Core.Telemetry
{
    public class PulseSensor : ISensor
    {
        private readonly Random _random;
        private readonly IPulseSensorOptions _sensorOptions;

        public PulseSensor(IPulseSensorOptions pulseSensorOptions)
        {
            _random = new Random();
            _sensorOptions = Ensure.Any.IsNotNull(pulseSensorOptions, nameof(pulseSensorOptions));
        }

        public int GetValue(DateTime currentTime)
        {
            var deterministicPart = GetDeterministicPart(currentTime);
            var randomPart = GetRandomPart();

            return deterministicPart + randomPart;
        }

        public void Report(DeviceTelemetry telemetry, DateTime time)
        {
            telemetry.Pulse = GetValue(time);
        }

        private int GetDeterministicPart(DateTime currentTime)
        {
            return (int)Math.Round(
                _sensorOptions.Function.GetValue(currentTime),
                MidpointRounding.AwayFromZero);
        }

        private int GetRandomPart()
        {
            return _random.Next(_sensorOptions.NoiseFactor * -1, _sensorOptions.NoiseFactor);
        }
    }
}
