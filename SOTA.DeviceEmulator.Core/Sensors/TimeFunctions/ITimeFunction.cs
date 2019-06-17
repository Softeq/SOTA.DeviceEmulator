using System;

namespace SOTA.DeviceEmulator.Core.Sensors.TimeFunctions
{
    public interface ITimeFunction<T>
    {
        T GetValue(DateTime time);
    }
}