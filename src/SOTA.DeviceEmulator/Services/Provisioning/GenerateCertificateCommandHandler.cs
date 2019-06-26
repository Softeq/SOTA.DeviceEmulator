using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace SOTA.DeviceEmulator.Services.Provisioning
{
    public class GenerateCertificateCommandHandler : IRequestHandler<GenerateCertificateCommand, string>
    {
        public GenerateCertificateCommandHandler()
        {

        }

        public Task<string> Handle(GenerateCertificateCommand request, CancellationToken cancellationToken)
        {
            var targetFileLocation = Directory.GetCurrentDirectory() + "//MyCert.pfx";

            using (var rsa = RSA.Create(2048))
            using (var rootCertificate = GetRootCertificateFromUserStore())
            using (var pureRootCertificate = RemovePrivateKey(rootCertificate))
            using (var newCertificate = CreateCertificate(rsa, rootCertificate))
            using (var newCertificateWithPrivateKey = newCertificate.CopyWithPrivateKey(rsa))
            {
                var certificates = new X509Certificate2Collection
                {
                    pureRootCertificate,
                    newCertificateWithPrivateKey
                };

                byte[] certData = certificates.Export(X509ContentType.Pkcs12, "sota");
                File.WriteAllBytes(targetFileLocation, certData);
            }

            return Task.FromResult(targetFileLocation);
        }

        private static X509Certificate2 GetRootCertificateFromUserStore()
        {
            using (var store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                store.Open(OpenFlags.ReadOnly);

                var rootCertificate = store.Certificates
                                           .Find(X509FindType.FindByThumbprint, "8B7C5620292D04DB01D55A60EB797F587AFC1620", false)[0];

                return rootCertificate;
            }
        }

        private static X509Certificate2 RemovePrivateKey(X509Certificate2 certificate)
        {
            var certificateData = certificate.Export(X509ContentType.Cert);
            var pureCertificate = new X509Certificate2(certificateData);

            return pureCertificate;
        }

        private static X509Certificate2 CreateCertificate(RSA rsa, X509Certificate2 issuerCertificate)
        {
            var certificateRequest = new CertificateRequest(
                $"CN=TestName-{Guid.NewGuid()}",
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

            var certificate = certificateRequest.Create(
                issuerCertificate,
                DateTimeOffset.UtcNow.AddDays(-1),
                issuerCertificate.NotAfter,
                Guid.NewGuid().ToByteArray());

            return certificate;
        }
    }
}
