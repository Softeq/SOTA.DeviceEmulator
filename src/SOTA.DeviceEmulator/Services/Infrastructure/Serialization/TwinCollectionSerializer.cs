using EnsureThat;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;

namespace SOTA.DeviceEmulator.Services.Infrastructure.Serialization
{
    internal class TwinCollectionSerializer : ITwinCollectionSerializer
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public TwinCollectionSerializer(JsonSerializerSettings jsonSerializerSettings)
        {
            _jsonSerializerSettings = Ensure.Any.IsNotNull(jsonSerializerSettings, nameof(jsonSerializerSettings));
        }

        public TwinCollection Serialize<T>(T value) where T : class
        {
            Ensure.Any.IsNotNull(value, nameof(value));

            var twinJson = JsonConvert.SerializeObject(value, _jsonSerializerSettings);
            var twinCollection = new TwinCollection(twinJson);
            return twinCollection;
        }

        public T Deserialize<T>(TwinCollection twinCollection) where T : class
        {
            Ensure.Any.IsNotNull(twinCollection, nameof(twinCollection));

            var twinJson = twinCollection.ToJson();
            return JsonConvert.DeserializeObject<T>(twinJson, _jsonSerializerSettings);
        }
    }
}
