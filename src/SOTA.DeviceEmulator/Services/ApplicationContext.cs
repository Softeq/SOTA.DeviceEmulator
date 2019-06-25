using Microsoft.Azure.Devices.Client;

namespace SOTA.DeviceEmulator.Services
{
    public class ApplicationContext : IApplicationContext
    {
        public DeviceClient DeviceClient { get; set; }
    }
}