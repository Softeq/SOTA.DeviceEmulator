using System;

namespace SOTA.DeviceEmulator.Core.Telemetry
{
    public class DeviceTelemetry
    {
        public DeviceTelemetry(Guid sessionId, DateTime timeStamp)
        {
            SessionId = sessionId;
            TimeStamp = timeStamp;
        }

        public Guid SessionId { get; }
        public DateTime TimeStamp { get; }
        public int Pulse { get; internal set; }
        public double Latitude { get; internal set; }
        public double Longitude { get; internal set; }
    }
}
