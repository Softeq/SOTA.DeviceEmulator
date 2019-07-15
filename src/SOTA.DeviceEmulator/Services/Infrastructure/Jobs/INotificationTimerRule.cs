using System;

namespace SOTA.DeviceEmulator.Services.Infrastructure.Jobs
{
    public interface INotificationTimerRule
    {
        TimeSpan Period { get; }

        void Trigger();
    }
}