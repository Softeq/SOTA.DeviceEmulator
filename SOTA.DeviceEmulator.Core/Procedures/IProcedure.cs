using System;

namespace SOTA.DeviceEmulator.Core.Procedures
{
    public interface IProcedure<T>
    {
        T GetValue(TimeSpan elapsedTime);
    }
}