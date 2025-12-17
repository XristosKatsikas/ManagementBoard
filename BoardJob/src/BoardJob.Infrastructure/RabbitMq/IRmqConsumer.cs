namespace BoardJob.Infrastructure.RabbitMq
{
    public interface IRmqConsumer
    {
        Task ConsumeAsync<T>(CancellationToken stoppingToken) where T : class;
    }
}