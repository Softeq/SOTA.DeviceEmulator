using System;
using EnsureThat;
using SOTA.DeviceEmulator.Core.Procedures;

namespace SOTA.DeviceEmulator.Core.Sensors
{
    public class PulseSensor : ISensor<int>
    {
        private readonly DateTime _startTime;
        private readonly IProcedure<double> _procedure;
        private readonly IClock _clock;
        private readonly Random _random;

        public PulseSensor(IProcedure<double> procedure, IClock clock)
        {
            _startTime = clock.UtcNow;

            _procedure = Ensure.Any.IsNotNull(procedure, nameof(procedure));
            _clock = Ensure.Any.IsNotNull(clock, nameof(clock));
            _random = new Random();
        }

        public int GetValue(int noiseFactor)
        {
            var deterministicPart = GetDeterministicPart();
            var randomPart = GetRandomPart(noiseFactor);

            return deterministicPart + randomPart;
        }

        private int GetDeterministicPart()
        {
            return (int)Math.Round(
                _procedure.GetValue(_clock.UtcNow - _startTime),
                MidpointRounding.AwayFromZero);
        }

        private int GetRandomPart(int noiseFactor)
        {
            return _random.Next(noiseFactor * -1, noiseFactor);
        }
    }
}
