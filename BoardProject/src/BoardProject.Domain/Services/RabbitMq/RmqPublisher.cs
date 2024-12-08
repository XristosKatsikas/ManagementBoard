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

        public RmqPublisher(EventBusSettings settings, ConnectionFactory eventBusConnectionFactory)
        {
            _settings = settings;
            _eventBusConnectionFactory = eventBusConnectionFactory;
            _connection = _eventBusConnectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: _settings.EventQueue, true, false);
        }

        public async Task PublishAsync<T>(T @event) where T : class
        {
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event));
            _channel.ConfirmSelect();
            await Task.Run(() => _channel.BasicPublish(exchange: "", routingKey: _settings.EventQueue, basicProperties: null, body: body));
            _channel.WaitForConfirmsOrDie();
        }

        public void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();
        }
    }
}
