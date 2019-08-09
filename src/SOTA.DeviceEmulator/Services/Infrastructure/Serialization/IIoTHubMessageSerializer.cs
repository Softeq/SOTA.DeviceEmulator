using Microsoft.Azure.Devices.Client;
using SOTA.DeviceEmulator.Core;

namespace SOTA.DeviceEmulator.Services.Infrastructure.Serialization
{
    internal interface IIoTHubMessageSerializer
    {
        Message Serialize<T>(T value) where T : class, ITemporalEvent;
    }
}