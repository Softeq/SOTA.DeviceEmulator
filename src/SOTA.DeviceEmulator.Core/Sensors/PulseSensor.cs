using System;
using SOTA.DeviceEmulator.Core.Sensors.TimeFunctions;

namespace SOTA.DeviceEmulator.Core.Sensors
{
    public class PulseSensor : ISensor
    {
        private readonly Random _random;
        private ITimeFunction<double> _function = new PulseHarmonicFunction();

        public PulseSensor()
        {
            _random = new Random();
        }

        public int NoiseFactor { get; set; } = 0;

        public ITimeFunction<double> Function
        {
            get => _function;
            set
            {
                if (value != null)
                {
                    _function = value;
                }
            }
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
                _function.GetValue(currentTime),
                MidpointRounding.AwayFromZero);
        }

        private int GetRandomPart()
        {
            return _random.Next(NoiseFactor * -1, NoiseFactor);
        }
    }
}
