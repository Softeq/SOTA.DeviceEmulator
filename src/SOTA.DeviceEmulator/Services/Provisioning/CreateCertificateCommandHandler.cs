using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using SOTA.DeviceEmulator.Core;

namespace SOTA.DeviceEmulator.Services.Provisioning
{
    public class CreateCertificateCommandHandler : IRequestHandler<CreateCertificateCommand, string>
    {
        private const string CertificatePassword = "sota";
        private readonly IConnectionOptions _connectionOptions;
        private readonly IDevice _device;

        public CreateCertificateCommandHandler(IConnectionOptions connectionOptions, IDevice device)
        {
            _connectionOptions = Ensure.Any.IsNotNull(connectionOptions, nameof(connectionOptions));
            _device = Ensure.Any.IsNotNull(device, nameof(device));
        }

        public Task<string> Handle(CreateCertificateCommand request, CancellationToken cancellationToken)
        {
            string targetFileLocation;

            using (var rsa = RSA.Create(2048))
            using (var rootCertificate = GetRootCertificateFromUserStore())
            using (var pureRootCertificate = RemovePrivateKey(rootCertificate))
            using (var newCertificate = CreateCertificate(rsa, rootCertificate))
            {
                targetFileLocation = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    _connectionOptions.CertificatesFolderName,
                    $"{newCertificate.SubjectName.Name}.pfx");

                Directory.CreateDirectory(Path.GetDirectoryName(targetFileLocation));

                var certificates = new X509Certificate2Collection
                {
                    pureRootCertificate,
                    newCertificate
                };

                byte[] certData = certificates.Export(X509ContentType.Pkcs12, CertificatePassword);
                File.WriteAllBytes(targetFileLocation, certData);
            }

            return Task.FromResult(targetFileLocation);
        }

        private X509Certificate2 GetRootCertificateFromUserStore()
        {
            using (var store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                store.Open(OpenFlags.ReadOnly);

                var certificates = store.Certificates.Find(
                    X509FindType.FindByThumbprint,
                    _connectionOptions.RootCertificateThumbprint,
                    false
                );

                if (certificates.Count == 0)
                {
                    throw new FileNotFoundException($"Certificate with thumbprint {_connectionOptions.RootCertificateThumbprint} not found. Please install it to the user store.");
                }

                return certificates[0];
            }
        }

        private static X509Certificate2 RemovePrivateKey(X509Certificate2 certificate)
        {
            var certificateData = certificate.Export(X509ContentType.Cert);
            var pureCertificate = new X509Certificate2(certificateData);

            return pureCertificate;
        }

        private X509Certificate2 CreateCertificate(RSA rsa, X509Certificate2 issuerCertificate)
        {
            var certificateRequest = new CertificateRequest(
                $"CN={_device.DisplayName}-{Guid.NewGuid()}",
                rsa,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);

            certificateRequest.CertificateExtensions.Add(
                new X509BasicConstraintsExtension(false, false, 0, false));

            // Key usage: server (1.3.6.1.5.5.7.3.1) and client (1.3.6.1.5.5.7.3.2) authentication
            certificateRequest.CertificateExtensions.Add(
                new X509EnhancedKeyUsageExtension(
                    new OidCollection
                    {
                        new Oid("1.3.6.1.5.5.7.3.1"),
                        new Oid("1.3.6.1.5.5.7.3.2")
                    },
                    true));

            using (var certificate = certificateRequest.Create(
                issuerCertificate,
                DateTimeOffset.UtcNow.AddDays(-1),
                issuerCertificate.NotAfter,
                Guid.NewGuid().ToByteArray()))
            {
                var certificateWithPrivateKey = certificate.CopyWithPrivateKey(rsa);

                return certificateWithPrivateKey;
            }
        }
    }
}
