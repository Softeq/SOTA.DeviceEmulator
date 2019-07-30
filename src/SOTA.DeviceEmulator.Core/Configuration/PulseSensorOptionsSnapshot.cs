using System;

#pragma warning disable 659

namespace SOTA.DeviceEmulator.Core.Configuration
{
    public class PulseSensorOptionsSnapshot : IEquatable<PulseSensorOptionsSnapshot>
    {
        public int NoiseFactor { get; set; }

        public string Algorithm { get; set; }

        public bool Equals(PulseSensorOptionsSnapshot other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return NoiseFactor == other.NoiseFactor && string.Equals(Algorithm, other.Algorithm);
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

            if (obj.GetType() != typeof(PulseSensorOptionsSnapshot))
            {
                return false;
            }

            return Equals((PulseSensorOptionsSnapshot)obj);
        }
    }
}