using System;

#pragma warning disable 659

namespace SOTA.DeviceEmulator.Core.Configuration
{
    public class TransmissionOptionsSnapshot : IEquatable<TransmissionOptionsSnapshot>
    {
        public bool Enabled { get; set; }

        public int Interval { get; set; }

        public bool Equals(TransmissionOptionsSnapshot other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Enabled == other.Enabled && Interval == other.Interval;
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

            return Equals((TransmissionOptionsSnapshot)obj);
        }
    }
}