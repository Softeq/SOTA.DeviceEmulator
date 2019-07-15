using GeoAPI.Geometries;

namespace SOTA.DeviceEmulator.Core.Telemetry.TimeFunctions
{
    public interface ILocationFunction : ITimeFunction<IPoint>
    {
        double Speed { get; set; }
    }
}