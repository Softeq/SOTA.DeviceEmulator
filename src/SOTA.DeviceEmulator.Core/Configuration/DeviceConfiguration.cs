using System;
using Newtonsoft.Json;
using SOTA.DeviceEmulator.Core.Telemetry.TimeFunctions;

#pragma warning disable 659

namespace SOTA.DeviceEmulator.Core.Configuration
{
    public class DeviceConfiguration : IEquatable<DeviceConfiguration>
    {
        [JsonConstructor]
        private DeviceConfiguration(
            TransmissionOptionsSnapshot transmission,
            LocationSensorOptionsSnapshot location,
            PulseSensorOptionsSnapshot pulse
        )
        {
            Transmission = transmission;
            Location = location;
            Pulse = pulse;
        }

        public TransmissionOptionsSnapshot Transmission { get; }

        public LocationSensorOptionsSnapshot Location { get; }

        public PulseSensorOptionsSnapshot Pulse { get; }

        public bool Equals(DeviceConfiguration other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(Transmission, other.Transmission) && Equals(Location, other.Location) &&
                   Equals(Pulse, other.Pulse);
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

            return Equals((DeviceConfiguration)obj);
        }

        public static DeviceConfiguration CreateDefault()
        {
            var transmission = new TransmissionOptionsSnapshot
            {
                Enabled = false,
                Interval = 3
            };
            var location = new LocationSensorOptionsSnapshot
            {
                SpeedMean = 5,
                SpeedDeviation = 1
            };
            var pulse = new PulseSensorOptionsSnapshot
            {
                NoiseFactor = 3,
                Algorithm = PulseHarmonicFunction.Name
            };
            return new DeviceConfiguration(transmission, location, pulse);
        }
    }
}