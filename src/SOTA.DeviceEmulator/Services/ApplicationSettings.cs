using System;
using System.Collections.Generic;
using System.Linq;
using EnsureThat;
using SOTA.DeviceEmulator.Services.Provisioning;

namespace SOTA.DeviceEmulator.Services.Settings
{
    public class ApplicationSettings : IConnectionOptions
    {
        private Dictionary<string, string> IdScopePerEnvironment => Properties
                                                                    .Settings.Default
                                                                    .DeviceProvisioningServiceIdScopeMap
                                                                    .Cast<string>()
                                                                    .Select(ParseKeyValuePair)
                                                                    .ToDictionary(x => x.Key, x => x.Value);

        private Dictionary<string, string> CertificateThumbprintPerEnvironment => Properties
                                                                                  .Settings.Default
                                                                                  .RootCertificateThumbprintMap
                                                                                  .Cast<string>()
                                                                                  .Select(ParseKeyValuePair)
                                                                                  .ToDictionary(x => x.Key, x => x.Value);

        public IReadOnlyCollection<string> Environments => IdScopePerEnvironment.Keys;

        public string DefaultEnvironment => Environments.Any(e => e == Properties.Settings.Default.DefaultEnvironment)
            ? Properties.Settings.Default.DefaultEnvironment
            : Environments.FirstOrDefault();

        public string DeviceProvisioningServiceEndpoint => Properties.Settings.Default.DeviceProvisioningServiceEndpoint;

        public string CertificatesFolderName => Properties.Settings.Default.CertificatesFolder;

        public string GetDeviceProvisioningServiceIdScope(string environment)
        {
            Ensure.String.IsNotNullOrEmpty(environment, nameof(environment));

            if (!IdScopePerEnvironment.ContainsKey(environment))
            {
                throw new InvalidOperationException($"Unknown environment: {environment}.");
            }
            return IdScopePerEnvironment[environment];
        }

        public string GetCertificateThumbprint(string environment)
        {
            Ensure.String.IsNotNullOrEmpty(environment, nameof(environment));

            if (!CertificateThumbprintPerEnvironment.ContainsKey(environment))
            {
                throw new InvalidOperationException($"Unknown environment: {environment}.");
            }
            return CertificateThumbprintPerEnvironment[environment];
        }

        private KeyValuePair<string, string> ParseKeyValuePair(string pair)
        {
            Ensure.String.IsNotNullOrEmpty(pair, nameof(pair), o => o.WithMessage($"Invalid key/value pair: '{pair}'."));
            var parts = pair.Split('=');
            var key = parts.FirstOrDefault();
            var value = parts.ElementAtOrDefault(1);
            return new KeyValuePair<string, string>(key, value);
        }
    }
}