using System;
using System.IO;
using System.Text;
using EnsureThat;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using SOTA.DeviceEmulator.Core;
using SOTA.DeviceEmulator.Services.Infrastructure.ModelMetadata;

namespace SOTA.DeviceEmulator.Services.Infrastructure.Serialization
{
    internal class IoTHubMessageSerializer : IIoTHubMessageSerializer
    {
        private readonly IModelMetadataProvider _modelMetadataProvider;
        private readonly JsonSerializer _jsonSerializer;
        private readonly UTF8Encoding _utf8Encoding;

        public IoTHubMessageSerializer(JsonSerializerSettings jsonSerializerSettings, IModelMetadataProvider modelMetadataProvider)
        {
            _modelMetadataProvider = Ensure.Any.IsNotNull(modelMetadataProvider, nameof(modelMetadataProvider));
            _jsonSerializer = JsonSerializer.Create(jsonSerializerSettings);
            _utf8Encoding = new UTF8Encoding(false);
        }

        public Message Serialize<T>(T value) where T : class, ITemporalEvent
        {
            var ms = new MemoryStream();
            try
            {
                using (var writer = new StreamWriter(ms, _utf8Encoding, bufferSize: 4096, leaveOpen: true))
                {
                    _jsonSerializer.Serialize(writer, value);
                }

                ms.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                var message = new Message(ms)
                {
                    CreationTimeUtc = value.TimeStamp,
                    ContentType = "application/json",
                    ContentEncoding = "utf-8",
                    Properties =
                    {
                        ["message-type"] = _modelMetadataProvider.GetTypeName(value.GetType())
                    }
                };
                return message;
            }
            catch (Exception)
            {
                ms.Dispose();
                throw;
            }
        }
    }
}
