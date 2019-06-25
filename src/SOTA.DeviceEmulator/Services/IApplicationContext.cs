using Microsoft.Azure.Devices.Client;

namespace SOTA.DeviceEmulator.Services
{
    public interface IApplicationContext
    {
        DeviceClient DeviceClient { get; set; }
    }
}
