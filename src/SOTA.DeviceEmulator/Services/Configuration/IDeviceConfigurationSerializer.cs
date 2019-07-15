using Microsoft.Azure.Devices.Shared;
using SOTA.DeviceEmulator.Core.Configuration;

namespace SOTA.DeviceEmulator.Services.Configuration
{
    internal interface IDeviceConfigurationSerializer
    {
        TwinCollection SerializeToDeviceProperties(DeviceConfiguration deviceConfiguration);
    }
}