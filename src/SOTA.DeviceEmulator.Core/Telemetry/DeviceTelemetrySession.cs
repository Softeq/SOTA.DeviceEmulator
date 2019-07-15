using System;
using EnsureThat;

namespace SOTA.DeviceEmulator.Core.Telemetry
{
    internal class DeviceTelemetrySession
    {
        private readonly IClock _clock;
        private bool _isTransmissionEnabled;
        private DateTime _startTime;

        public DeviceTelemetrySession(IClock clock, bool isTransmissionEnabled)
        {
            _clock = Ensure.Any.IsNotNull(clock, nameof(clock));
            _isTransmissionEnabled = isTransmissionEnabled;
            Id = Guid.NewGuid();
            _startTime = _clock.UtcNow;
        }

        public Guid Id { get; private set; }

        public TimeSpan GetElapsedTime(DateTime now)
        {
            Ensure.Comparable.IsGte(now, _startTime, nameof(now));

            if (!_isTransmissionEnabled)
            {
                return TimeSpan.Zero;
            }
            return now - _startTime;
        }

        public void Toggle(bool isTransmissionEnabled)
        {
            if (_isTransmissionEnabled == isTransmissionEnabled)
            {
                return;
            }
            Id = Guid.NewGuid();
            _startTime = _clock.UtcNow;
            _isTransmissionEnabled = isTransmissionEnabled;
        }
    }
}
