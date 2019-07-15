using System;
using EnsureThat;
using Microsoft.Azure.Devices.Provisioning.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SOTA.DeviceEmulator.Core;
using SOTA.DeviceEmulator.Core.Configuration;
using SOTA.DeviceEmulator.Services.Infrastructure.Serialization;

namespace SOTA.DeviceEmulator.Services
{
    internal class DevicePropertiesSerializer : IDevicePropertiesSerializer
    {
        private readonly DefaultContractResolver _contractResolver;
        private readonly JsonSerializerSettings _jsonSerializerSettings;
        private readonly ITwinCollectionSerializer _twinCollectionSerializer;

        public DevicePropertiesSerializer(
            JsonSerializerSettings jsonSerializerSettings,
            ITwinCollectionSerializer twinCollectionSerializer
        )
        {
            _jsonSerializerSettings = jsonSerializerSettings;
            _twinCollectionSerializer = Ensure.Any.IsNotNull(
                twinCollectionSerializer,
                nameof(twinCollectionSerializer)
            );
            _contractResolver = jsonSerializerSettings.ContractResolver as DefaultContractResolver;
            if (_contractResolver == null)
            {
                throw new ArgumentException(
                    "Expected JSON contract resolver to be derived from default one.",
                    nameof(jsonSerializerSettings)
                );
            }
        }

        public DeviceProperties Deserialize(TwinCollection twinCollection)
        {
            Ensure.Any.IsNotNull(twinCollection, nameof(twinCollection));

            return _twinCollectionSerializer.Deserialize<DeviceProperties>(twinCollection);
        }

        public TwinCollection SerializeToDeviceProperties(DeviceConfiguration deviceConfiguration)
        {
            Ensure.Any.IsNotNull(deviceConfiguration, nameof(deviceConfiguration));

            return SerializeSection(nameof(DeviceProperties.Configuration), deviceConfiguration);
        }

        public TwinCollection SerializeToDeviceProperties(DeviceInformation deviceInformation)
        {
            Ensure.Any.IsNotNull(deviceInformation, nameof(deviceInformation));

            return SerializeSection(nameof(DeviceProperties.Information), deviceInformation);
        }

        public ProvisioningRegistrationAdditionalData SerializeToRegistrationData(DeviceInformation information)
        {
            Ensure.Any.IsNotNull(information, nameof(information));

            var tagsJson = JsonConvert.SerializeObject(information, _jsonSerializerSettings);
            return new ProvisioningRegistrationAdditionalData
            {
                JsonData = tagsJson
            };
        }

        private TwinCollection SerializeSection<T>(string name, T section) where T : class
        {
            var sectionName =
                _contractResolver.GetResolvedPropertyName(name);
            var configurationTwinCollection = _twinCollectionSerializer.Serialize(section);
            var twinCollection = new TwinCollection
            {
                [sectionName] = configurationTwinCollection
            };
            return twinCollection;
        }
    }
}