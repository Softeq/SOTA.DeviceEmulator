using System;

namespace SOTA.DeviceEmulator.Core
{
    public interface ITemporalEvent
    {
        DateTime TimeStamp { get; }
    }
}
