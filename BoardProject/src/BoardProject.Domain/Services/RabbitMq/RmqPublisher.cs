using BoardProject.Domain.Configurations;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
using System.Text;

namespace BoardProject.Domain.Services.RabbitMq
{
    public class RmqPublisher : IRmqPublisher
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly EventBusSettings _settings;

        public RmqPublisher(EventBusSettings settings, ConnectionFactory eventBusConnectionFactory)
        {
            _connection = eventBusConnectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _settings = settings;

            // Declare exchange
            _channel.ExchangeDeclare(exchange: _settings.Fanout, type: ExchangeType.Fanout);

            // Enable publisher confirms
            _channel.ConfirmSelect();
        }

        public async Task<T> PublishAsync<T>(object @event) where T : class
        {
            var consumer = new EventingBasicConsumer(_channel);
            var tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
            var correlationId = Guid.NewGuid().ToString();

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var responseString = Encoding.UTF8.GetString(body);
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    var data = JsonSerializer.Deserialize<T>(responseString);
                    tcs.SetResult(data!);
                }
            };

            var messageBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));
            var basicProperties = _channel.CreateBasicProperties();
            basicProperties.CorrelationId = correlationId;
            basicProperties.ReplyTo = _settings.EventQueue;
            _channel.BasicPublish(exchange: _settings.Fanout, routingKey: "", basicProperties: basicProperties, body: messageBody);

            return await tcs.Task;
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            _channel?.Dispose();
            _connection?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}