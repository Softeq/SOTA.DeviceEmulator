using System;
using MediatR;

namespace SOTA.DeviceEmulator.Services.Infrastructure.Jobs
{
    public interface INotificationTimerRule
    {
        TimeSpan Period { get; }

        INotification CreateNotification();
    }
}