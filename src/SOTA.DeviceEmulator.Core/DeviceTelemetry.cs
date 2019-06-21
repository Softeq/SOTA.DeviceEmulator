using GeoAPI.Geometries;

namespace SOTA.DeviceEmulator.Core
{
    public class DeviceTelemetry
    {
        public int Pulse { get; internal set; }
        public double Latitude { get; internal set; }
        public double Longitude { get; internal set; }
    }
}
