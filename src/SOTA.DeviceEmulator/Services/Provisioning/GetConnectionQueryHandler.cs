using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using SOTA.DeviceEmulator.Core;

namespace SOTA.DeviceEmulator.Services.Provisioning
{
    public class GetConnectionQueryHandler : IRequestHandler<GetConnectionQuery, ConnectionModel>
    {
        private readonly IDevice _device;

        public GetConnectionQueryHandler(IDevice device)
        {
            _device = Ensure.Any.IsNotNull(device, nameof(device));
        }

        public Task<ConnectionModel> Handle(GetConnectionQuery request, CancellationToken cancellationToken)
        {
            var connection = new ConnectionModel(_device.DisplayName, _device.IsConnected);
            return Task.FromResult(connection);
        }
    }
}