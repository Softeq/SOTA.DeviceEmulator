using System;
using GeoAPI.Geometries;
using SOTA.DeviceEmulator.Core.Sensors.TimeFunctions;

namespace SOTA.DeviceEmulator.Core.Sensors
{
    public class LocationSensor : ISensor
    {
        private readonly Random _random;
        private ILocationFunction _function = new LocationHarmonicFunction();

        public LocationSensor()
        {
            _random = new Random();
        }

        public ILocationFunction Function
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

        // Speed mean and deviation in km/h.
        public double SpeedMean { get; set; } = 5;
        public double SpeedDeviation { get; set; } = 0;

        public IPoint GetValue(DateTime currentTime)
        {
            var functionResult = _function.GetValue(currentTime);

            var point = EarthGeometry.GeometryFactory.CreatePoint(new Coordinate(functionResult.Coordinate.X, functionResult.Coordinate.Y));

            return point;
        }

        public void Report(DeviceTelemetry telemetry, DateTime time)
        {
            _function.Speed = CalculateSpeed(SpeedMean, SpeedDeviation);
            var point = GetValue(time);

            telemetry.Latitude = point.Y;
            telemetry.Longitude = point.X;
        }

        private double CalculateSpeed(double mean, double deviation)
        {
            // Operations with 10 are required to get random double value with single precision sign.
            var speedDeviation = _random.Next((int)Math.Floor(-10 * deviation), (int)Math.Floor(10 * deviation)) / 10;
            var speed = mean + speedDeviation;

            return speed;
        }
    }
}
