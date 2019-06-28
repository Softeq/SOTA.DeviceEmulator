using System;
using System.Collections.Generic;
using System.Text;
using EnsureThat;
using SOTA.DeviceEmulator.Core.Sensors;

namespace SOTA.DeviceEmulator.Core
{
    public class Device : IDevice
    {
        private readonly object _lock = new object();
        private readonly IDeviceState _deviceState;
        private readonly List<ISensor> _sensors;
        private readonly IClock _clock;
        private DeviceConnectionMetadata _connectionMetadata;
        private DateTime? _sessionStartTime;
        private DateTime? _lastTransmissionTime;

        public Device(IDeviceState deviceState, IClock clock)
        {
            _deviceState = Ensure.Any.IsNotNull(deviceState, nameof(deviceState));
            _clock = Ensure.Any.IsNotNull(clock, nameof(clock));

            _sensors = new List<ISensor>
            {
                new PulseSensor(_deviceState.PulseSensorOptions),
                new LocationSensor(_deviceState.LocationSensorOptions)
            };

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

        public DeviceTelemetryReport GetTelemetryReport()
        {
            var telemetry = new DeviceTelemetry();
            var now = _clock.UtcNow;
            _sensors.ForEach(sensor => sensor.Report(telemetry, now));

            UpdateTimes(now);

            var isNeedToTransmit = CheckIfNeedToTransmit(now);

            if (isNeedToTransmit)
            {
                _lastTransmissionTime = now;
            }

            var report = new DeviceTelemetryReport
            {
                Telemetry = telemetry,
                IsNeedToTransmit = isNeedToTransmit
            };

            return report;
        }

        private void UpdateTimes(DateTime now)
        {
            if (_deviceState.IsTransmissionEnabled)
            {
                if (_sessionStartTime == null)
                {
                    _sessionStartTime = now;
                }

                _deviceState.SessionTime = now - (DateTime)_sessionStartTime;
            }
            else
            {
                _deviceState.SessionTime = TimeSpan.Zero;
                _sessionStartTime = null;
            }
        }

        private bool CheckIfNeedToTransmit(DateTime now)
        {
            var elapsedSinceLastTransmission = now - ( _lastTransmissionTime ?? DateTime.MinValue );

            return elapsedSinceLastTransmission > TimeSpan.FromSeconds(_deviceState.TransmissionPeriod) &&
                _deviceState.IsTransmissionEnabled;
        }
    }
}
