using Microsoft.Azure.Devices.Shared;
using SOTA.DeviceEmulator.Services.Configuration;
using SOTA.DeviceEmulator.Services.Provisioning;

namespace SOTA.DeviceEmulator.Services
{
    internal interface IDevicePropertiesSerializer : IDeviceConfigurationSerializer, IDeviceInformationSerializer
    {
        TwinCollection Serialize(DeviceProperties deviceProperties);
        DeviceProperties Deserialize(TwinCollection twinCollection);
    }
}