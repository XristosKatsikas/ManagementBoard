namespace BoardProject.Domain.Services.RabbitMq
{
    public interface IMessagePublisher : IDisposable
    {
        Task PublishAsync<T>(T @event);
    }
}
