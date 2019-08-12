using System;

namespace SOTA.DeviceEmulator.Core.Telemetry
{
    public class DeviceTelemetry : ITemporalEvent
    {
        public DeviceTelemetry(Guid sessionId, DateTime timeStamp)
        {
            SessionId = sessionId;
            TimeStamp = timeStamp;
        }

        public Guid SessionId { get; }
        // TODO: Ignore field from JSON as it's also sent as system header
        public DateTime TimeStamp { get; }
        public int Pulse { get; internal set; }
        public double Latitude { get; internal set; }
        public double Longitude { get; internal set; }
    }
}
