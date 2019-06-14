using System;

namespace SOTA.DeviceEmulator.Core
{
    public interface IClock
    {
        DateTime UtcNow { get; }
    }
}