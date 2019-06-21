using System;

namespace SOTA.DeviceEmulator.Core.Sensors.TimeFunctions
{
    public interface ITimeFunction<T>
    {
        string DisplayName { get; }
        T GetValue(DateTime time);
    }
}