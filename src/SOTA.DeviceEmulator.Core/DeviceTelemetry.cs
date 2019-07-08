using System;

namespace SOTA.DeviceEmulator.Core
{
    public class DeviceTelemetry
    {
        public string DeviceId { get; internal set; }
        public Guid SessionId { get; internal set; }
        public DateTime TimeStamp { get; set; }
        public int Pulse { get; internal set; }
        public double Latitude { get; internal set; }
        public double Longitude { get; internal set; }
    }
}
