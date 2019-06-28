namespace SOTA.DeviceEmulator.Core
{
    public class DeviceTelemetryReport
    {
        public DeviceTelemetry Telemetry { get; set; }

        public bool IsNeedToTransmit { get; set; }
    }
}
