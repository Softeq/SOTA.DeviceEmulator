using System;
using SOTA.DeviceEmulator.Core;

namespace SOTA.DeviceEmulator.Services.Infrastructure.Jobs
{
    internal class SystemClock : IClock
    {
        private static readonly Lazy<SystemClock> InstanceLazy = new Lazy<SystemClock>(() => new SystemClock());

        public static SystemClock Instance => InstanceLazy.Value;

        public DateTime UtcNow => DateTime.UtcNow;

        private SystemClock()
        {
        }
    }
}
