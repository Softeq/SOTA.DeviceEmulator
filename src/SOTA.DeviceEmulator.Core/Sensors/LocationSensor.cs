using System;
using EnsureThat;
using GeoAPI.Geometries;
using SOTA.DeviceEmulator.Core.Sensors.TimeFunctions;

namespace SOTA.DeviceEmulator.Core.Sensors
{
    public class LocationSensor : ISensor
    {
        private readonly Random _random;
        private readonly ILocationSensorOptions _sensorOptions;
        private ILocationFunction _function = new LocationHarmonicFunction();

        public LocationSensor(ILocationSensorOptions locationSensorOptions)
        {
            _random = new Random();
            _sensorOptions = Ensure.Any.IsNotNull(locationSensorOptions, nameof(locationSensorOptions));
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

        public IPoint GetValue(DateTime currentTime)
        {
            var functionResult = _function.GetValue(currentTime);

            var point = EarthGeometry.GeometryFactory.CreatePoint(new Coordinate(functionResult.Coordinate.X, functionResult.Coordinate.Y));

            return point;
        }

        public void Report(DeviceTelemetry telemetry, DateTime time)
        {
            _function.Speed = CalculateSpeed(_sensorOptions.SpeedMean, _sensorOptions.SpeedDeviation);
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
