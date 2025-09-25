using BoardProject.Domain.Configurations;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace BoardProject.Domain.Services.RabbitMq
{
    public class RmqPublisher<T> : IRmqPublisher<T>
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly EventBusSettings _settings;

        private readonly ConcurrentDictionary<string, TaskCompletionSource<T>> _pendingResponses = new();

        //Use a thread-safe dictionary to track pending requests
        private readonly ConcurrentDictionary<ulong, TaskCompletionSource<bool>> _pendingConfirms = new();

        public RmqPublisher(EventBusSettings settings, ConnectionFactory eventBusConnectionFactory)
        {
            _connection = eventBusConnectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _settings = settings;

            _channel.ExchangeDeclare(exchange: _settings.Fanout, type: ExchangeType.Fanout);
            // enable publisher for acknowledgments
            _channel.ConfirmSelect();

            _channel.BasicAcks += OnBasicAck!;
            _channel.BasicNacks += OnBasicNack!;
            // replyConsumer listens for responses on a specific queue, process incoming messages,
            // and complete associated tasks with the response data
            var replyConsumer = new AsyncEventingBasicConsumer(_channel);
            //Subscribes to the Received event, which fires whenever a message arrives.
            //The handler is async to allow awaiting asynchronous operations inside.
            replyConsumer.Received += async (model, ea) =>
            {
                var correlationId = ea.BasicProperties.CorrelationId;
                if (_pendingResponses.TryRemove(correlationId, out var tcs))
                {
                    try
                    {
                        var responseString = Encoding.UTF8.GetString(ea.Body.ToArray());
                        // responseData is the deserialized response message received via RabbitMQ
                        var responseData = JsonSerializer.Deserialize<T>(responseString);
                        if (responseData is not null)
                        {
                            // set the result of the TaskCompletionSource<T> (tcs) to responseData
                            tcs.SetResult(responseData);
                        }
                        else
                        {
                            tcs.SetException(new Exception("Response deserialized to null."));
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log unexpected errors
                        tcs.SetException(ex);
                    }
                }
                await Task.CompletedTask;
            };
            _channel.BasicConsume(queue: _settings.EventQueue, autoAck: true, consumer: replyConsumer);
        }

        public async Task<T> PublishAsync(object @event)
        {
            var correlationId = Guid.NewGuid().ToString();
            var confirmTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            try
            {
                _pendingConfirms[_channel.NextPublishSeqNo] = confirmTcs;

                var messageBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));
                var basicProperties = _channel.CreateBasicProperties();
                basicProperties.CorrelationId = correlationId;
                basicProperties.ReplyTo = _settings.EventQueue;

                _channel.BasicPublish(exchange: _settings.Fanout, routingKey: "", basicProperties: basicProperties, body: messageBody);

                await confirmTcs.Task; // Wait for broker confirmation

                var responseTcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
                _pendingResponses[correlationId] = responseTcs;

                return await responseTcs.Task; // Waits asynchronously for response
            }
            catch (Exception ex)
            {
                // log error
                throw;
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

        /// <summary>
        /// Completes the tasks waiting for publisher confirms, signaling that the message has been successfully acknowledged.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ea"></param>
        private void OnBasicAck(object sender, BasicAckEventArgs ea)
        {
            // Handle multiple confirmations
            if (ea.Multiple)
            {
                var confirmedSeqNos = _pendingConfirms.Keys.Where(seq => seq <= ea.DeliveryTag).ToList();
                foreach (var seq in confirmedSeqNos)
                {
                    if (_pendingConfirms.TryRemove(seq, out var tcs))
                    {
                        tcs.SetResult(true);
                    }
                }
            }
            else
            {
                if (_pendingConfirms.TryRemove(ea.DeliveryTag, out var tcs))
                {
                    tcs.SetResult(true);
                }
            }
        }

        /// <summary>
        /// Completes the tasks waiting for publisher confirms with an exception, 
        /// signaling the message was negatively acknowledged by the broker.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ea"></param>
        private void OnBasicNack(object sender, BasicNackEventArgs ea)
        {
            // Handle multiple nacks
            if (ea.Multiple)
            {
                var nackedSeqNos = _pendingConfirms.Keys.Where(seq => seq <= ea.DeliveryTag).ToList();
                foreach (var seq in nackedSeqNos)
                {
                    if (_pendingConfirms.TryRemove(seq, out var tcs))
                    {
                        tcs.SetException(new Exception("Message nacked by broker."));
                    }
                }
            }
            else
            {
                if (_pendingConfirms.TryRemove(ea.DeliveryTag, out var tcs))
                {
                    tcs.SetException(new Exception("Message nacked by broker."));
                }
            }
        }
    }
}