namespace SOTA.DeviceEmulator.Core
{
    public interface ITransmissionOptions
    {
        bool Enabled { get; }

        int Interval { get; }
    }
}
