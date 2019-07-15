using Microsoft.Azure.Devices.Shared;

namespace SOTA.DeviceEmulator.Services.Infrastructure.Serialization
{
    internal interface ITwinCollectionSerializer
    {
        TwinCollection Serialize<T>(T value) where T : class;
        T Deserialize<T>(TwinCollection twinCollection) where T : class;
    }
}