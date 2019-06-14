namespace SOTA.DeviceEmulator.Services.Infrastructure.Jobs
{
    public interface IToggleable
    {
        bool IsEnabled { get; }
    }
}
