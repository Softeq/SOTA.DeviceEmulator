using System.Collections.Generic;
using FluentAssertions;
using Moq;
using SOTA.DeviceEmulator.Core.Configuration;
using SOTA.DeviceEmulator.Core.Telemetry.TimeFunctions;
using SOTA.DeviceEmulator.Core.Tests.Stubs;
using Xunit;

namespace SOTA.DeviceEmulator.Core.Tests
{
    public class DeviceTest
    {
        private readonly Mock<IEventPublisher> _eventPublisherMock;
        private readonly Device _device;

        public DeviceTest()
        {
            var clock = new TestClock();
            _eventPublisherMock = new Mock<IEventPublisher>(MockBehavior.Strict);
            _eventPublisherMock.Setup(x => x.Publish(It.IsAny<object>()));
            var doubleTimeFunctionMock = new Mock<ITimeFunction<double>>(MockBehavior.Strict);
            doubleTimeFunctionMock.SetupGet(x => x.DisplayName).Returns("MockDoubleTimeFunction");
            var doubleTimeFunctions = new List<ITimeFunction<double>>
            {
                doubleTimeFunctionMock.Object
            };
            _device = new Device(clock, _eventPublisherMock.Object, doubleTimeFunctions);
        }

        [Fact]
        public void TryGetChangedConfiguration_ReturnsTrue_WhenConfigurationWasChangedFromThePreviousCall()
        {
            var initialChanged = _device.TryGetChangedConfiguration(out var initialConfiguration);
            initialChanged.Should().BeTrue();

            _device.Configuration.Transmission.Enabled = true;
            _device.Configuration.Location.SpeedMean = 10;

            var changed = _device.TryGetChangedConfiguration(out var changedConfiguration);
            changed.Should().BeTrue();
            initialConfiguration.Should().NotBeEquivalentTo(changedConfiguration);
        }

        [Fact]
        public void TryGetChangedConfiguration_ReturnsFalse_WhenConfigurationWasUnchangedFromThePreviousCall()
        {
            var initialChanged = _device.TryGetChangedConfiguration(out _);
            initialChanged.Should().BeTrue();

            var previousEnabled = _device.Configuration.Transmission.Enabled;
            var previousMean = _device.Configuration.Location.SpeedMean;

            _device.Configuration.Transmission.Enabled = !previousEnabled;
            _device.Configuration.Location.SpeedMean = previousMean * 2;
            _device.Configuration.Transmission.Enabled = previousEnabled;
            _device.Configuration.Location.SpeedMean = previousMean;

            var changed = _device.TryGetChangedConfiguration(out var changedConfiguration);
            changed.Should().BeFalse();
            changedConfiguration.Should().BeNull();
        }

        [Fact]
        public void ChangeConfigurationProperty_RaisesConfigurationChangedEvent()
        {
            _device.Configuration.Location.SpeedMean = _device.Configuration.Location.SpeedMean * 2;
            _eventPublisherMock.Verify(x => x.Publish(It.IsAny<DeviceConfigurationChanged>()), Times.Once);
        }

        [Fact]
        public void ChangeConfigurationPropertyToTheSameValue_DoesNotRaiseConfigurationChangedEvent()
        {
            _device.Configuration.Location.SpeedMean = _device.Configuration.Location.SpeedMean;
            _eventPublisherMock.Verify(x => x.Publish(It.IsAny<DeviceConfigurationChanged>()), Times.Never);
        }

        [Fact]
        public void Connect_DoesNotAcceptInvalidConfiguration()
        {
            var configuration = DeviceConfiguration.CreateDefault();
            configuration.Transmission.Interval = -5;
            configuration.Location.SpeedMean = _device.Configuration.Location.SpeedMean * 2;
            var deviceMetadata = new DeviceMetadata(
                "some-device",
                "some-device",
                desiredConfiguration: configuration,
                reportedConfiguration: null
            );
            var validationResult = _device.Connect(deviceMetadata);
            validationResult.IsValid.Should().BeFalse();
            _device.Configuration.Transmission.Interval.Should().NotBe(configuration.Transmission.Interval);
            _device.Configuration.Location.SpeedMean.Should().NotBe(configuration.Location.SpeedMean);
        }
    }
}