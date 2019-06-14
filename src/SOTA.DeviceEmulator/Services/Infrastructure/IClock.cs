using System;

namespace SOTA.DeviceEmulator.Services.Infrastructure
{
    public interface IClock
    {
        DateTime UtcNow { get; }
    }
}