using Microsoft.Azure.Devices.Shared;

namespace SOTA.DeviceEmulator.Services.Configuration
{
    public class DeviceConfigurationRemotelyUpdated
    {
        public TwinCollection TwinCollection { get; set; }
    }
}
