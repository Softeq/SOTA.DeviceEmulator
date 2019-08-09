using System;
using EnsureThat;
using SOTA.DeviceEmulator.Core.Telemetry;

namespace SOTA.DeviceEmulator.Services.Infrastructure.ModelMetadata
{
    public class ModelMetadataProvider : IModelMetadataProvider
    {
        public string GetTypeName(Type type)
        {
            Ensure.Any.IsNotNull(type, nameof(type));

            if (type == typeof(DeviceTelemetry))
            {
                return "SofteqEmulatorTelemetry";
            }
            throw new NotSupportedException($"No model metadata found for type {type.Name}.");
        }
    }
}
