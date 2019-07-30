namespace SOTA.DeviceEmulator.Core
{
    public interface IEventPublisher
    {
        void Publish<T>(T @event) where T : class;
    }
}
