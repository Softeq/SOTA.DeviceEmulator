using System;

#pragma warning disable 659

namespace SOTA.DeviceEmulator.Core.Configuration
{
    public class LocationSensorOptionsSnapshot : IEquatable<LocationSensorOptionsSnapshot>
    {
        public double SpeedMean { get; set; }

        public double SpeedDeviation { get; set; }

        public bool Equals(LocationSensorOptionsSnapshot other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return SpeedMean.Equals(other.SpeedMean) && SpeedDeviation.Equals(other.SpeedDeviation);
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

            return Equals((LocationSensorOptionsSnapshot)obj);
        }
    }
}