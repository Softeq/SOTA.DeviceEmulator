using FluentValidation;

namespace SOTA.DeviceEmulator.Core.Configuration
{
    public class DeviceConfigurationValidator : AbstractValidator<DeviceConfiguration>
    {
        public DeviceConfigurationValidator()
        {
            RuleFor(x => x.Transmission.Interval).GreaterThanOrEqualTo(1);
            RuleFor(x => x.Location.SpeedMean).InclusiveBetween(1, 50);
            RuleFor(x => x.Location.SpeedDeviation)
                .GreaterThanOrEqualTo(0).LessThanOrEqualTo(x => x.Location.SpeedMean);
            RuleFor(x => x.Pulse.Algorithm).Equal("Harmonic");
            RuleFor(x => x.Pulse.NoiseFactor).InclusiveBetween(0, 30);
        }
    }
}