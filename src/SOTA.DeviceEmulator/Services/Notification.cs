using EnsureThat;
using MediatR;

namespace SOTA.DeviceEmulator.Services
{
    public class Notification<T> : INotification where T : class
    {
        public Notification(T @event)
        {
            Event = Ensure.Any.IsNotNull(@event, nameof(@event));
        }

        public T Event { get; }
    }
}
