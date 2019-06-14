namespace SOTA.DeviceEmulator.Core.Sensors
{
    public interface ISensor<T>
    {
        T GetValue(int noiseFactor);
    }
}