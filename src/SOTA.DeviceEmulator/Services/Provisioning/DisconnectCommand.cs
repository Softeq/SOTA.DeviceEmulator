using MediatR;

namespace SOTA.DeviceEmulator.Services.Provisioning
{
    public class DisconnectCommand : IRequest<ConnectionModel>
    {
    }
}