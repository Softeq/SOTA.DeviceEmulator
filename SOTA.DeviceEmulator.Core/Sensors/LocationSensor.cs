using System;
using EnsureThat;
using GeoAPI.Geometries;
using SOTA.DeviceEmulator.Core.Sensors.TimeFunctions;

namespace SOTA.DeviceEmulator.Core.Sensors
{
    public class LocationSensor
    {
        // Noise step in kilometers.
        public const double NoiseStep = 0.1;
        public int NoiseFactor = 1;

        private readonly ITimeFunction<IPoint> _function;
        private readonly Random _random;

        public LocationSensor(ITimeFunction<IPoint> function)
        {
            _function = Ensure.Any.IsNotNull(function, nameof(function));
            _random = new Random();
        }

        public IPoint GetValue(DateTime currentTime)
        {
            var deterministicPart = _function.GetValue(currentTime);

            var noiseDistanceLat = _random.Next(NoiseFactor * -1, NoiseFactor) * NoiseStep;
            var noiseDistanceLon = _random.Next(NoiseFactor * -1, NoiseFactor) * NoiseStep;

            var pointWithNoise = EarthGeometry.GeometryFactory.CreatePoint(
                new Coordinate(
                    deterministicPart.Coordinate.X + EarthGeometry.ConvertDistanceToAngleSize(noiseDistanceLon),
                    deterministicPart.Coordinate.Y + EarthGeometry.ConvertDistanceToAngleSize(noiseDistanceLat)));

            return pointWithNoise;
        }
    }
}
