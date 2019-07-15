using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using EnsureThat;
using FluentValidation.Results;
using Force.DeepCloner;
using SOTA.DeviceEmulator.Core.Configuration;
using SOTA.DeviceEmulator.Core.Telemetry;
using SOTA.DeviceEmulator.Core.Telemetry.TimeFunctions;

namespace SOTA.DeviceEmulator.Core
{
    internal class DeviceState : IDeviceConfiguration, IDeviceConfigurationHolder
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly object _deviceLock;
        private DeviceConfiguration _snapshot;
        private DeviceConfiguration _previousSnapshot;
        private readonly DeviceConfigurationValidator _configurationValidator;

        public DeviceState(
            IClock clock,
            IEventPublisher eventPublisher,
            IEnumerable<ITimeFunction<double>> doubleTimeFunctions,
            object deviceLock
        )
        {
            _configurationValidator = new DeviceConfigurationValidator();
            _eventPublisher = Ensure.Any.IsNotNull(eventPublisher, nameof(eventPublisher));
            Ensure.Any.IsNotNull(clock, nameof(clock));
            Ensure.Any.IsNotNull(doubleTimeFunctions, nameof(doubleTimeFunctions));
            _deviceLock = Ensure.Any.IsNotNull(deviceLock, nameof(deviceLock));

            _snapshot = DeviceConfiguration.CreateDefault();
            Session = new DeviceTelemetrySession(clock, _snapshot.Transmission.Enabled);
            Transmission = new TransmissionOptions(this);
            Location = new LocationSensorOptions(this);
            Pulse = new PulseSensorOptions(this, doubleTimeFunctions);
        }

        public DeviceTelemetrySession Session { get; }
        public ITransmissionOptions Transmission { get; }
        public ILocationSensorOptions Location { get; }
        public IPulseSensorOptions Pulse { get; }

        public ValidationResult UpdateConfiguration(DeviceConfiguration configuration)
        {
            Ensure.Any.IsNotNull(configuration, nameof(configuration));

            var validationResult = _configurationValidator.Validate(configuration);
            if (!validationResult.IsValid)
            {
                return validationResult;
            }
            lock (_deviceLock)
            {
                if (_snapshot.Equals(configuration))
                {
                    return validationResult;
                }
                _snapshot = configuration.DeepClone();
                Session.Toggle(_snapshot.Transmission.Enabled);
                OnConfigurationChanged();
                return validationResult;
            }
        }

        public bool TryGetChangedConfiguration(out DeviceConfiguration deviceConfiguration)
        {
            deviceConfiguration = null;
            lock (_deviceLock)
            {
                if (_previousSnapshot == null)
                {
                    _previousSnapshot = _snapshot.DeepClone();
                    deviceConfiguration = _previousSnapshot;
                    return true;
                }
                if (_previousSnapshot.Equals(_snapshot))
                {
                    return false;
                }
                _previousSnapshot = _snapshot.DeepClone();
            }
            deviceConfiguration = _previousSnapshot;
            return true;
        }

        public void Set<T>(Expression<Func<DeviceConfiguration, T>> selector, T value)
        {
            Ensure.Any.IsNotNull(selector, nameof(selector));
            if (!( selector.Body is MemberExpression memberExpression ))
            {
                throw new ArgumentException("Expected selector body to be a member expression.", nameof(selector));
            }

            if (!( memberExpression.Member is PropertyInfo valueProperty ))
            {
                throw new ArgumentException("Expected selector to reference a property.", nameof(selector));
            }

            object GetTarget(Expression expression)
            {
                switch (expression.NodeType)
                {
                    case ExpressionType.Parameter:
                        // ReSharper disable once InconsistentlySynchronizedField
                        return _snapshot;
                    case ExpressionType.MemberAccess:
                        var member = (MemberExpression)expression;
                        var property = member.Member as PropertyInfo;
                        if (property == null)
                        {
                            throw new InvalidOperationException($"Expected {member.Member.Name} to be a property.");
                        }

                        var target = GetTarget(member.Expression);
                        return property.GetValue(target, null);
                    default:
                        throw new InvalidOperationException($"Unexpected expression node type: {expression.NodeType}.");
                }
            }

            var compiledSelector = selector.Compile();
            lock (_deviceLock)
            {
                var previousValue = compiledSelector(_snapshot);
                if (EqualityComparer<T>.Default.Equals(previousValue, value))
                {
                    return;
                }
                var target = GetTarget(memberExpression.Expression);
                valueProperty.SetValue(target, value, null);
                Session.Toggle(_snapshot.Transmission.Enabled);
                OnConfigurationChanged();
            }
        }

        public T Get<T>(Func<DeviceConfiguration, T> selector)
        {
            Ensure.Any.IsNotNull(selector, nameof(selector));

            lock (_deviceLock)
            {
                return selector(_snapshot);
            }
        }

        private void OnConfigurationChanged()
        {
            _eventPublisher.Publish(new DeviceConfigurationChanged());
        }
    }
}