using System;

namespace SOTA.DeviceEmulator.Core.Telemetry
{
    public interface ISensor
    {
        void Report(DeviceTelemetry telemetry, DateTime time);
    }
}