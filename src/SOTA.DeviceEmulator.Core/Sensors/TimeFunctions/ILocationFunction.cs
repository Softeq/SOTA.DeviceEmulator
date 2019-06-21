using GeoAPI.Geometries;

namespace SOTA.DeviceEmulator.Core.Sensors.TimeFunctions
{
    public interface ILocationFunction : ITimeFunction<IPoint>
    {
        double Speed { get; set; }
    }
}