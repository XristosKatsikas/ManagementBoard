using BoardProject.Domain.Configurations;
using BoardProject.Domain.DTOs.Responses;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
using System.Collections.Concurrent;
using System.Text;

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
            //_channel.BasicAcks += (sender, ea) =>
            //{
            //    if (_pendingConfirms.TryRemove(ea.DeliveryTag, out var tcs))
            //    {
            //        tcs.TrySetResult(true); // Confirmed
            //        _unconfirmedMessages.TryRemove(ea.DeliveryTag, out _);
            //    }
            //};

            //// Handle NACKs
            //_channel.BasicNacks += (sender, ea) =>
            //{
            //    if (_pendingConfirms.TryRemove(ea.DeliveryTag, out var tcs))
            //    {
            //        tcs.TrySetException(new Exception("Message was NACKed by broker."));
            //        _unconfirmedMessages.TryRemove(ea.DeliveryTag, out _);
            //    }
            //};
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

            // var deliveryTag = _channel.NextPublishSeqNo;
            // Store message and confirmation task
            //_unconfirmedMessages.TryAdd(deliveryTag, @event!);
            //_pendingConfirms.TryAdd(deliveryTag, tcs);

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