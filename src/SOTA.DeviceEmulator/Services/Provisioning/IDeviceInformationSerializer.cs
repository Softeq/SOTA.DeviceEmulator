using Microsoft.Azure.Devices.Provisioning.Client;
using SOTA.DeviceEmulator.Core;

namespace SOTA.DeviceEmulator.Services.Provisioning
{
    internal interface IDeviceInformationSerializer
    {
        ProvisioningRegistrationAdditionalData SerializeToRegistrationData(DeviceInformation information);
    }
}