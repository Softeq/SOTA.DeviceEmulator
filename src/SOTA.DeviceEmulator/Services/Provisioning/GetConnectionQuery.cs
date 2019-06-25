using MediatR;

namespace SOTA.DeviceEmulator.Services.Provisioning
{
    public class GetConnectionQuery : IRequest<ConnectionModel>
    {
    }
}
