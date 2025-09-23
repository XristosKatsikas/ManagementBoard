using BoardProject.Domain.Configurations;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Channels;

namespace BoardProject.Domain.Services.RabbitMq
{
    public class RmqPublisher : IRmqPublisher
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ConcurrentDictionary<ulong, object> _unconfirmedMessages = new ConcurrentDictionary<ulong, object>();
        private readonly ConcurrentDictionary<ulong, TaskCompletionSource<bool>> _pendingConfirms = new ConcurrentDictionary<ulong, TaskCompletionSource<bool>>();
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

            // Handle ACKs
            _channel.BasicAcks += (sender, ea) =>
            {
                if (_pendingConfirms.TryRemove(ea.DeliveryTag, out var tcs))
                {
                    tcs.TrySetResult(true); // Confirmed
                    _unconfirmedMessages.TryRemove(ea.DeliveryTag, out _);
                }
            };

            // Handle NACKs
            _channel.BasicNacks += (sender, ea) =>
            {
                if (_pendingConfirms.TryRemove(ea.DeliveryTag, out var tcs))
                {
                    tcs.TrySetException(new Exception("Message was NACKed by broker."));
                    _unconfirmedMessages.TryRemove(ea.DeliveryTag, out _);
                }
            };
        }

        public async Task PublishAsync<T>(T @event) where T : class
        {
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event));
            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            var deliveryTag = _channel.NextPublishSeqNo;

            var basicProperties = _channel.CreateBasicProperties();
            basicProperties.CorrelationId = Guid.NewGuid().ToString();
            basicProperties.ReplyTo = _settings.EventQueue;

            // Store message and confirmation task
            _unconfirmedMessages.TryAdd(deliveryTag, @event!);
            _pendingConfirms.TryAdd(deliveryTag, tcs);

            // Publish message
            _channel.BasicPublish(exchange: _settings.Fanout, routingKey: "", basicProperties: basicProperties, body: body);

            // Await broker confirmation
            await tcs.Task;
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