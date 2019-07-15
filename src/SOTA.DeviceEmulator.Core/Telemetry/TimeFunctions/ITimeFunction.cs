using System;

namespace SOTA.DeviceEmulator.Core.Telemetry.TimeFunctions
{
    public interface ITimeFunction<T>
    {
        string DisplayName { get; }
        T GetValue(DateTime time);
    }
}