using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnsureThat;
using SOTA.DeviceEmulator.Core.Sensors;

namespace SOTA.DeviceEmulator.Core
{
    public class Device : IDevice
    {
        private readonly object _lock = new object();
        private readonly List<ISensor> _sensors;
        private readonly IClock _clock;
        private DeviceConnectionMetadata _connectionMetadata;


        public Device(IEnumerable<ISensor> sensors, IClock clock)
        {
            _sensors = Ensure.Any.IsNotNull(sensors, nameof(sensors)).ToList();
            _clock = Ensure.Any.IsNotNull(clock, nameof(clock));
            Metadata = new DeviceMetadata();
        }

        public string DisplayName
        {
            get
            {
                lock (_lock)
                {
                    var builder = new StringBuilder(Metadata.UserName);
                    builder.Append("-emulator");
                    if (_connectionMetadata != null)
                    {
                        builder.Append($" ({_connectionMetadata.DeviceId})");
                    }
                    return builder.ToString();
                }
            }
        }

        public bool IsConnected => _connectionMetadata != null;

        public void Connect(DeviceConnectionMetadata connectionMetadata)
        {
            Ensure.Any.IsNotNull(connectionMetadata, nameof(connectionMetadata));

            lock (_lock)
            {
                _connectionMetadata = connectionMetadata;
            }
            
        }

        public void Disconnect()
        {
            lock (_lock)
            {
                _connectionMetadata = null;
            }
        }

        public DeviceMetadata Metadata { get; }

        public DeviceTelemetry ReportTelemetry()
        {
            var telemetry = new DeviceTelemetry();
            var time = _clock.UtcNow;
            _sensors.ForEach(sensor => sensor.Report(telemetry, time));
            return telemetry;
        }
    }
}
