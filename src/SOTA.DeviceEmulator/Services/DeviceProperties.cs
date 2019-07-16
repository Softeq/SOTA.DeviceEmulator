using System;
using SOTA.DeviceEmulator.Core;
using SOTA.DeviceEmulator.Core.Configuration;

namespace SOTA.DeviceEmulator.Services
{
    internal class DeviceProperties : IEquatable<DeviceProperties>
    {
        public bool Equals(DeviceProperties other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(Information, other.Information) && Equals(Configuration, other.Configuration);
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((DeviceProperties)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ( ( Information != null ? Information.GetHashCode() : 0 ) * 397 ) ^
                       ( Configuration != null ? Configuration.GetHashCode() : 0 );
            }
        }

        public DeviceProperties(DeviceInformation information, DeviceConfiguration configuration)
        {
            Information = information;
            Configuration = configuration;
        }

        public DeviceInformation Information { get; }

        public DeviceConfiguration Configuration { get; }
    }
}