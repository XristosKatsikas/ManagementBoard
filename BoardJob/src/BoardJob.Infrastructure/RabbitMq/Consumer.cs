using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;
using BoardJob.Domain.Configurations;
using MediatR;

namespace BoardJob.Infrastructure.RabbitMq
{
    public class Consumer
    {
        private readonly IConnection _connection;
        private readonly IMediator _mediator;
        private readonly EventBusSettings _settings;
        private readonly IModel? _channel;

        public Consumer(IMediator mediator, EventBusSettings settings, ConnectionFactory factory)
        {
            _settings = settings;
            _connection = factory.CreateConnection();
            _mediator = mediator;
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection.Dispose();
        }

        public Task ExecuteAsync<T>(CancellationToken stoppingToken) where T : class
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                var @event = JsonConvert.DeserializeObject<T>(content);

                await _mediator.Send(@event!, stoppingToken);
                _channel?.BasicAck(ea.DeliveryTag, false);
            };

            consumer.Model?.QueueDeclare(_settings.EventQueue, true, false);
            _channel?.BasicConsume(_settings.EventQueue, false, consumer);

            return Task.CompletedTask;
        }
    }
}