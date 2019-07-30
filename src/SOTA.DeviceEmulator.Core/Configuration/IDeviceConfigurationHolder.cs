using System;
using System.Linq.Expressions;

namespace SOTA.DeviceEmulator.Core.Configuration
{
    internal interface IDeviceConfigurationHolder
    {
        void Set<T>(Expression<Func<DeviceConfiguration, T>> selector, T value);
        T Get<T>(Func<DeviceConfiguration, T> selector);
    }
}