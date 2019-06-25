using System;

namespace SOTA.DeviceEmulator.Core
{
    public class DeviceMetadata
    {
        public string MachineName => Environment.MachineName;
        public string UserName => Environment.UserName;
        public bool IsEmulator => true;
    }
}
