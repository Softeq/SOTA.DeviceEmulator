using System;
using System.IO;
using System.Text;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

namespace SOTA.DeviceEmulator.Services.Infrastructure.Serialization
{
    internal class IoTHubMessageSerializer : IIoTHubMessageSerializer
    {
        private readonly JsonSerializer _jsonSerializer;

        public IoTHubMessageSerializer(JsonSerializerSettings jsonSerializerSettings)
        {
            _jsonSerializer = JsonSerializer.Create(jsonSerializerSettings);
        }

        public Message Serialize<T>(T value) where T : class
        {
            var ms = new MemoryStream();
            try
            {
                using (var writer = new StreamWriter(ms, Encoding.UTF8, bufferSize: 4096, leaveOpen: true))
                {
                    _jsonSerializer.Serialize(writer, value);
                }
                return new Message(ms);
            }
            catch (Exception)
            {
                ms.Dispose();
                throw;
            }
        }
    }
}
