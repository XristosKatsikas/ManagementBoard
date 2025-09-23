namespace BoardProject.Domain.Services.RabbitMq
{
    public interface IRmqPublisher : IDisposable
    {
        Task PublishAsync<T>(T @event) where T : class;
    }
}
