using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Serilog;
using SOTA.DeviceEmulator.Core;

namespace SOTA.DeviceEmulator.Services.Provisioning
{
    public class DisconnectCommandHandler : IRequestHandler<DisconnectCommand, ConnectionModel>
    {
        private readonly IApplicationContext _applicationContext;
        private readonly IDevice _device;
        private readonly ILogger _logger;

        public DisconnectCommandHandler(IDevice device, IApplicationContext applicationContext, ILogger logger)
        {
            _device = Ensure.Any.IsNotNull(device, nameof(device));
            _logger = Ensure.Any.IsNotNull(logger, nameof(logger)).ForContext(GetType());
            _applicationContext = Ensure.Any.IsNotNull(applicationContext, nameof(applicationContext));
        }

        public async Task<ConnectionModel> Handle(DisconnectCommand request, CancellationToken cancellationToken)
        {
            if (_applicationContext.DeviceClient == null)
            {
                return new ConnectionModel(_device.DisplayName, false);
            }

            _device.Disconnect();
            await _applicationContext.DeviceClient.CloseAsync(cancellationToken);
            _applicationContext.DeviceClient.Dispose();
            _applicationContext.DeviceClient = null;
            _logger.Information("Device is disconnected.");
            return new ConnectionModel(_device.DisplayName, false);
        }
    }
}