using System;

namespace SOTA.DeviceEmulator.Services.Infrastructure.ModelMetadata
{
    internal interface IModelMetadataProvider
    {
        string GetTypeName(Type type);
    }
}
