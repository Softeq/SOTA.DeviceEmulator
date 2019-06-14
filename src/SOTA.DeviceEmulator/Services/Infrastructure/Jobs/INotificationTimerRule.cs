using System;
using Caliburn.Micro;

namespace SOTA.DeviceEmulator.Services.Infrastructure.Jobs
{
    public interface INotificationTimerRule
    {
        TimeSpan Period { get; }

        void Trigger();
    }
}