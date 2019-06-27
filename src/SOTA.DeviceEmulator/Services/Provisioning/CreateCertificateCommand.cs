using MediatR;

namespace SOTA.DeviceEmulator.Services.Provisioning
{
    public class CreateCertificateCommand : IRequest<string>
    {
        public string Environment { get; set; }
    }
}
