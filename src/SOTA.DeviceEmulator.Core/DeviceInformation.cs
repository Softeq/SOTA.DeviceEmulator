using System;

namespace SOTA.DeviceEmulator.Core
{
    public class DeviceInformation : IEquatable<DeviceInformation>
    {
        public string Vendor { get; } = "Softeq";
        public string Model { get; } = "Emulator";
        public string MachineName { get; } = Environment.MachineName;
        public string UserName { get; } = Environment.UserName;

        public bool Equals(DeviceInformation other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return string.Equals(Vendor, other.Vendor) && string.Equals(Model, other.Model) &&
                   string.Equals(MachineName, other.MachineName) && string.Equals(UserName, other.UserName);
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

            if (obj.GetType() != typeof(DeviceInformation))
            {
                return false;
            }

            return Equals((DeviceInformation)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Vendor.GetHashCode();
                hashCode = ( hashCode * 397 ) ^ Model.GetHashCode();
                hashCode = ( hashCode * 397 ) ^ MachineName.GetHashCode();
                hashCode = ( hashCode * 397 ) ^ UserName.GetHashCode();
                return hashCode;
            }
        }
    }
}