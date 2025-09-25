namespace BoardProject.Domain.Services.RabbitMq
{
    public interface IRmqPublisher<T> : IDisposable
    {
        Task<T> PublishAsync(object @event);
    }
}