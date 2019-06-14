using System;
using EnsureThat;
using GeoAPI.Geometries;
using SOTA.DeviceEmulator.Core.Procedures;

namespace SOTA.DeviceEmulator.Core.Sensors
{
    public class LocationSensor : ISensor<IPoint>
    {
        // Noise step in kilometers.
        private const double NoiseStep = 0.1;

        private readonly DateTime _startTime;
        private readonly IProcedure<IPoint> _procedure;
        private readonly IClock _clock;
        private readonly Random _random;

        public LocationSensor(IProcedure<IPoint> procedure, IClock clock)
        {
            _startTime = clock.UtcNow;

            _procedure = Ensure.Any.IsNotNull(procedure, nameof(procedure));
            _clock = Ensure.Any.IsNotNull(clock, nameof(clock));
            _random = new Random();
        }

        public IPoint GetValue(int noiseFactor)
        {
            var deterministicPart = _procedure.GetValue(_clock.UtcNow - _startTime);

            var noiseDistanceLat = _random.Next(noiseFactor * -1, noiseFactor) * NoiseStep;
            var noiseDistanceLon = _random.Next(noiseFactor * -1, noiseFactor) * NoiseStep;

            var pointWithNoise = EarthGeometry.GeometryFactory.CreatePoint(
                new Coordinate(
                    deterministicPart.Coordinate.X + EarthGeometry.ConvertDistanceToAngleSize(noiseDistanceLon),
                    deterministicPart.Coordinate.Y + EarthGeometry.ConvertDistanceToAngleSize(noiseDistanceLat)));

            return pointWithNoise;
        }
    }
}
