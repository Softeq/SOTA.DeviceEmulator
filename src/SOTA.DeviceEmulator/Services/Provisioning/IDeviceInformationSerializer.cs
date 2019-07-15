using Microsoft.Azure.Devices.Provisioning.Client;
using Microsoft.Azure.Devices.Shared;
using SOTA.DeviceEmulator.Core;

namespace SOTA.DeviceEmulator.Services.Provisioning
{
    internal interface IDeviceInformationSerializer
    {
        TwinCollection SerializeToDeviceProperties(DeviceInformation deviceInformation);

        ProvisioningRegistrationAdditionalData SerializeToRegistrationData(DeviceInformation information);
    }
}