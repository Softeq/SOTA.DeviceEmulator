namespace SOTA.DeviceEmulator.Core.Configuration
{
    public interface ITransmissionOptions
    {
        bool Enabled { get; set; }

        int Interval { get; set; }
    }
}
