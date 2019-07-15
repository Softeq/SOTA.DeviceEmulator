using SOTA.DeviceEmulator.Core;
using SOTA.DeviceEmulator.Core.Configuration;

namespace SOTA.DeviceEmulator.Services
{
    internal class DeviceProperties
    {
        public DeviceProperties(DeviceInformation information, DeviceConfiguration configuration)
        {
            Information = information;
            Configuration = configuration;
        }

        public DeviceInformation Information { get; }

        public DeviceConfiguration Configuration { get; }
    }
}
