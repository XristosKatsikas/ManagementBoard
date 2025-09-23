namespace BoardJob.Infrastructure.RabbitMq
{
    public interface IRmqConsumer
    {
        Task ExecuteAsync<T>(CancellationToken stoppingToken) where T : class;
    }
}