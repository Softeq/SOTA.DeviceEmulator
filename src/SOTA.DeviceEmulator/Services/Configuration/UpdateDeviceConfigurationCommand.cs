using MediatR;
using Microsoft.Azure.Devices.Shared;

namespace SOTA.DeviceEmulator.Services.Configuration
{
    public class UpdateDeviceConfigurationCommand : IRequest
    {
        public TwinCollection TwinCollection { get; set; }
    }
}
