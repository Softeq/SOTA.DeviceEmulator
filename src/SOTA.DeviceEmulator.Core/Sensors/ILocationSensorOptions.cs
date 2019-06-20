namespace SOTA.DeviceEmulator.Core.Sensors
{
    public interface ILocationSensorOptions
    {
        double SpeedMean { get; } 
        double SpeedDeviation { get; }
    }
}