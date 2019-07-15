using Microsoft.Azure.Devices.Client;

namespace SOTA.DeviceEmulator.Services.Infrastructure.Serialization
{
    internal interface IIoTHubMessageSerializer
    {
        Message Serialize<T>(T value) where T : class;
    }
}