using BoardProject.Domain.Configurations;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace BoardProject.Domain.Services.RabbitMq
{
    public class RmqPublisher<T> : IRmqPublisher<T>, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly EventBusSettings _settings;

        // tracks responses for request-response messaging.
        // It's essential if you're expecting responses to correlate with requests.
        private readonly ConcurrentDictionary<string, TaskCompletionSource<T>> _pendingResponses = new();
        //tracks message confirmations (acks/nacks) from RabbitMQ.
        //It's necessary for publisher confirm handling, to know whether messages were successfully published.
        private readonly ConcurrentDictionary<ulong, TaskCompletionSource<bool>> _pendingConfirms = new();

        private AsyncEventingBasicConsumer? _replyConsumer;

        public RmqPublisher(EventBusSettings settings, ConnectionFactory connectionFactory)
        {
            _settings = settings;
            _connection = connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();

            InitializeExchangeAndQueue();
            SetupConsumer();
        }

        private void InitializeExchangeAndQueue()
        {
            // Declare exchange
            _channel.ExchangeDeclare(exchange: _settings.Fanout, type: ExchangeType.Fanout);

            // Declare queue with dead-letter exchange
            var args = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", "x-dead-letter-exchange" } // ensure this exists in settings
            };

            _channel.QueueDeclare(queue: _settings.EventQueue, durable: true, exclusive: false, autoDelete: false, arguments: args);

            // Bind queue to exchange if needed
            // _channel.QueueBind(_settings.EventQueue, _settings.Fanout, routingKey: "");
        }

        private void SetupConsumer()
        {
            _channel.ConfirmSelect(); // enable publisher confirms
            _channel.BasicAcks += OnBasicAck!;
            _channel.BasicNacks += OnBasicNack!;

            _replyConsumer = new AsyncEventingBasicConsumer(_channel);
            _replyConsumer.Received += async (model, ea) =>
            {
                var correlationId = ea.BasicProperties.CorrelationId;
                if (_pendingResponses.TryRemove(correlationId, out var tcs))
                {
                    try
                    {
                        var responseString = Encoding.UTF8.GetString(ea.Body.ToArray());
                        var responseData = JsonSerializer.Deserialize<T>(responseString);
                        if (responseData != null)
                        {
                            tcs.SetResult(responseData);
                        }
                        else
                        {
                            tcs.SetException(new Exception("Response deserialized to null."));
                        }
                    }
                    catch (Exception ex)
                    {
                        tcs.SetException(ex);
                    }
                }
                await Task.CompletedTask;
            };

            _channel.BasicConsume(queue: _settings.EventQueue, autoAck: true, consumer: _replyConsumer);
        }

        public async Task<T> PublishAsync(object @event)
        {
            var correlationId = Guid.NewGuid().ToString();
            var confirmTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            _pendingConfirms[_channel.NextPublishSeqNo] = confirmTcs;

            var messageBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));
            var basicProperties = _channel.CreateBasicProperties();
            basicProperties.CorrelationId = correlationId;
            basicProperties.ReplyTo = _settings.EventQueue;

            _channel.BasicPublish(
                exchange: _settings.Fanout,
                routingKey: "",
                basicProperties: basicProperties,
                body: messageBody);

            await confirmTcs.Task; // wait for broker confirmation

            var responseTcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
            _pendingResponses[correlationId] = responseTcs;

            return await responseTcs.Task;
        }

        private void OnBasicAck(object sender, BasicAckEventArgs ea)
        {
            HandleConfirms(ea.DeliveryTag, ea.Multiple, success: true);
        }

        private void OnBasicNack(object sender, BasicNackEventArgs ea)
        {
            HandleConfirms(ea.DeliveryTag, ea.Multiple, success: false);
        }

        private void HandleConfirms(ulong deliveryTag, bool multiple, bool success)
        {
            if (multiple)
            {
                var seqNos = _pendingConfirms.Keys.Where(seq => seq <= deliveryTag).ToList();
                foreach (var seq in seqNos)
                {
                    if (_pendingConfirms.TryRemove(seq, out var tcs))
                    {
                        if (success)
                            tcs.SetResult(true);
                        else
                            tcs.SetException(new Exception("Message nacked by broker."));
                    }
                }
            }
            else
            {
                if (_pendingConfirms.TryRemove(deliveryTag, out var tcs))
                {
                    if (success)
                        tcs.SetResult(true);
                    else
                        tcs.SetException(new Exception("Message nacked by broker."));
                }
            }
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