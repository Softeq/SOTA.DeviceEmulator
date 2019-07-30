using System;
using System.Collections.Generic;
using System.Text;
using EnsureThat;
using FluentValidation.Results;
using SOTA.DeviceEmulator.Core.Configuration;
using SOTA.DeviceEmulator.Core.Telemetry;
using SOTA.DeviceEmulator.Core.Telemetry.TimeFunctions;

namespace SOTA.DeviceEmulator.Core
{
    public class Device : IDevice
    {
        private readonly IClock _clock;
        private readonly DeviceState _deviceState;
        private readonly object _lock = new object();
        private readonly List<ISensor> _sensors;
        private DateTime? _lastTransmissionTime;
        private DeviceMetadata _metadata;

        public Device(IClock clock, IEventPublisher eventPublisher, IEnumerable<ITimeFunction<double>> doubleTimeFunctions)
        {
            Ensure.Any.IsNotNull(eventPublisher, nameof(eventPublisher));
            Ensure.Any.IsNotNull(doubleTimeFunctions, nameof(doubleTimeFunctions));
            _clock = Ensure.Any.IsNotNull(clock, nameof(clock));
            _deviceState = new DeviceState(clock, eventPublisher, doubleTimeFunctions, _lock);
            _sensors = new List<ISensor>
            {
                new PulseSensor(_deviceState.Pulse),
                new LocationSensor(_deviceState.Location)
            };
            Information = new DeviceInformation();
        }

        public DeviceInformation Information { get; }

        public string DisplayName
        {
            get
            {
                lock (_lock)
                {
                    var builder = new StringBuilder(Information.UserName);
                    builder.Append("-emulator");
                    if (_metadata != null)
                    {
                        builder.Append($" ({_metadata.DeviceId})");
                    }
                    return builder.ToString();
                }
            }
        }

        public IDeviceConfiguration Configuration => _deviceState;

        public bool IsConnected => _metadata != null;

        public ValidationResult Connect(DeviceMetadata metadata)
        {
            Ensure.Any.IsNotNull(metadata, nameof(metadata));

            var configuration = metadata.DesiredConfiguration ?? metadata.ReportedConfiguration;
            var validationResult = configuration != null
                ? _deviceState.UpdateConfiguration(configuration)
                : new ValidationResult();
            lock (_lock)
            {
                _metadata = metadata;
            }
            return validationResult;
        }

        public ValidationResult UpdateConfiguration(DeviceConfiguration configuration)
        {
            Ensure.Any.IsNotNull(configuration, nameof(configuration));

            var validationResult = _deviceState.UpdateConfiguration(configuration);
            return validationResult;
        }

        public void Disconnect()
        {
            lock (_lock)
            {
                _metadata = null;
            }
        }

        public DeviceTelemetryReport GetTelemetryReport()
        {
            lock (_lock)
            {
                var now = _clock.UtcNow;
                var telemetry = new DeviceTelemetry(_deviceState.Session.Id, now);
                _sensors.ForEach(sensor => sensor.Report(telemetry, now));
                var isPublished = IsPublished(telemetry);
                var report = new DeviceTelemetryReport(
                    telemetry,
                    isPublished,
                    _deviceState.Session.GetElapsedTime(now)
                );
                return report;
            }
        }

        public bool TryGetChangedConfiguration(out DeviceConfiguration deviceConfiguration)
        {
            return _deviceState.TryGetChangedConfiguration(out deviceConfiguration);
        }

        private bool IsPublished(DeviceTelemetry telemetry)
        {
            if (!IsConnected)
            {
                return false;
            }

            if (!_deviceState.Transmission.Enabled)
            {
                return false;
            }

            var elapsedSinceLastTransmission = telemetry.TimeStamp - ( _lastTransmissionTime ?? DateTime.MinValue );
            var isPublished = elapsedSinceLastTransmission > TimeSpan.FromSeconds(_deviceState.Transmission.Interval);
            if (isPublished)
            {
                _lastTransmissionTime = telemetry.TimeStamp;
            }

            return isPublished;
        }
    }
}