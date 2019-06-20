using System.Collections.Generic;
using System.Linq;
using EnsureThat;
using SOTA.DeviceEmulator.Core.Sensors;

namespace SOTA.DeviceEmulator.Core
{
    public class Device : IDevice
    {
        private readonly List<ISensor> _sensors;
        private readonly IClock _clock;

        public PulseSensor PulseSensor => _sensors.OfType<PulseSensor>().FirstOrDefault();
        public LocationSensor LocationSensor => _sensors.OfType<LocationSensor>().FirstOrDefault();

        public Device(IEnumerable<ISensor> sensors, IClock clock)
        {
            _sensors = Ensure.Any.IsNotNull(sensors, nameof(sensors)).ToList();
            _clock = Ensure.Any.IsNotNull(clock, nameof(clock));

            Ensure.Any.IsNotNull(PulseSensor, nameof(PulseSensor));
            Ensure.Any.IsNotNull(LocationSensor, nameof(LocationSensor));
        }

        public DeviceTelemetry ReportTelemetry()
        {
            var telemetry = new DeviceTelemetry();
            var time = _clock.UtcNow;

            _sensors.ForEach(sensor => sensor.Report(telemetry, time));

            return telemetry;
        }
    }
}
