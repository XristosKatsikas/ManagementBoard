using BoardProject.Domain.Configurations;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace BoardProject.Domain.Services.RabbitMq
{
    public class RmqPublisher
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ConnectionFactory _eventBusConnectionFactory;
        private readonly EventBusSettings _settings;

        private const string fanoutExchange = "fanout";

        public RmqPublisher(EventBusSettings settings, ConnectionFactory eventBusConnectionFactory)
        {
            _settings = settings;
            _eventBusConnectionFactory = eventBusConnectionFactory;
            _connection = _eventBusConnectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            // Declare the fanout exchange if not exists
            _channel.ExchangeDeclare(exchange: fanoutExchange, type: ExchangeType.Fanout);
            // Enable publisher confirms
            _channel.ConfirmSelect();
        }

        public async Task PublishAsync<T>(T @event) where T : class
        {
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event));

            await Task.Run(() =>
            {
                _channel.BasicPublish(
                    exchange: fanoutExchange,
                    routingKey: "",
                    basicProperties: null,
                    body: body);
            });

            // Wait for confirmation
            try
            {
                _channel.WaitForConfirmsOrDie();
            }
            catch (Exception ex)
            {
                // Handle exception (logging, retries, etc.)
                throw new ApplicationException("Message publish failed.", ex);
            }
        }

        public void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();
        }
    }
}