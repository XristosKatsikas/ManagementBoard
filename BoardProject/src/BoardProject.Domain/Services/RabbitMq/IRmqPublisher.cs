namespace BoardProject.Domain.Services.RabbitMq
{
    public interface IRmqPublisher : IDisposable
    {
        Task<T> PublishAsync<T>(object @event) where T : class;
    }
}
