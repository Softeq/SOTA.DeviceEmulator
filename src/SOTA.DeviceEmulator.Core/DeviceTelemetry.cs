using System;
using GeoAPI.Geometries;

namespace SOTA.DeviceEmulator.Core
{
    public class DeviceTelemetry
    {
        public Guid SessionId { get; internal set; }
        public DateTime TimeStamp { get; set; }
        public int Pulse { get; internal set; }
        public double Latitude { get; internal set; }
        public double Longitude { get; internal set; }
    }
}
