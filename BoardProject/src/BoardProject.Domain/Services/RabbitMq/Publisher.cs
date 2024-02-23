using BoardProject.Domain.Configurations;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace BoardProject.Domain.Services.RabbitMq
{
    public class Publisher
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ConnectionFactory _eventBusConnectionFactory;
        private readonly EventBusSettings _settings;

        public Publisher(EventBusSettings settings, ConnectionFactory eventBusConnectionFactory)
        {
            _settings = settings;
            _eventBusConnectionFactory = eventBusConnectionFactory;
            _connection = _eventBusConnectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: _settings.EventQueue, true, false);
        }

        public void PublishMessage<T>(T message) where T : class
        {
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            _channel.ConfirmSelect();
            _channel.BasicPublish(exchange: "", routingKey: "", basicProperties: null, body: body);
            _channel.WaitForConfirmsOrDie();
        }

        public void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();
        }
    }
}
