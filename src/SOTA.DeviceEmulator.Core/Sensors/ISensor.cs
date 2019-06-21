using System;

namespace SOTA.DeviceEmulator.Core.Sensors
{
    public interface ISensor
    {
        void Report(DeviceTelemetry telemetry, DateTime time);
    }
}