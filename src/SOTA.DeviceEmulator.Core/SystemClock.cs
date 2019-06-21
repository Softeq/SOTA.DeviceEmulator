using System;

namespace SOTA.DeviceEmulator.Core
{
    public class SystemClock : IClock
    {
        private static readonly Lazy<SystemClock> InstanceLazy = new Lazy<SystemClock>(() => new SystemClock());

        public static SystemClock Instance => InstanceLazy.Value;

        public DateTime UtcNow => DateTime.UtcNow;

        private SystemClock()
        {
        }
    }
}
