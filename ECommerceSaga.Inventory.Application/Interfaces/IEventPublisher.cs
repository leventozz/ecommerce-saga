namespace ECommerceSaga.Inventory.Application.Interfaces
{
    public interface IEventPublisher
    {
        Task Publish<T>(T message, CancellationToken cancellationToken = default) where T : class;
    }
}
