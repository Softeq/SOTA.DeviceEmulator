using Microsoft.Azure.Devices.Shared;

namespace SOTA.DeviceEmulator.Services.Configuration
{
    public class DeviceConfigurationUpdated
    {
        public TwinCollection TwinCollection { get; set; }
    }
}
