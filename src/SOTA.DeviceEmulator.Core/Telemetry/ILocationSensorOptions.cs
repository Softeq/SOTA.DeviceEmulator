namespace SOTA.DeviceEmulator.Core.Telemetry
{
    public interface ILocationSensorOptions
    {
        double SpeedMean { get; set; } 
        double SpeedDeviation { get; set; }
    }
}