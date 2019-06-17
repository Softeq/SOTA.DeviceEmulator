using System;
using EnsureThat;
using SOTA.DeviceEmulator.Core.Sensors.TimeFunctions;

namespace SOTA.DeviceEmulator.Core.Sensors
{
    public class PulseSensor
    {
        public int NoiseFactor = 5;

        private readonly ITimeFunction<double> _function;
        private readonly Random _random;

        public PulseSensor(ITimeFunction<double> function)
        {
            _function = Ensure.Any.IsNotNull(function, nameof(function));
            _random = new Random();
        }

        public int GetValue(DateTime currentTime)
        {
            var deterministicPart = GetDeterministicPart(currentTime);
            var randomPart = GetRandomPart();

            return deterministicPart + randomPart;
        }

        private int GetDeterministicPart(DateTime currentTime)
        {
            return (int)Math.Round(
                _function.GetValue(currentTime),
                MidpointRounding.AwayFromZero);
        }

        private int GetRandomPart()
        {
            return _random.Next(NoiseFactor * -1, NoiseFactor);
        }
    }
}
