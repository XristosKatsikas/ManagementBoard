using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;
using BoardJob.Domain.Configurations;
using MediatR;

namespace BoardJob.Infrastructure.RabbitMq
{
    public class RmqConsumer : IRmqConsumer
    {
        private readonly IConnection _connection;
        private readonly IMediator _mediator;
        private readonly EventBusSettings _settings;
        private readonly IModel? _channel;

        public RmqConsumer(IMediator mediator, EventBusSettings settings, ConnectionFactory factory)
        {
            _settings = settings;
            _connection = factory.CreateConnection();
            _mediator = mediator;
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(_settings.EventQueue, durable: false, exclusive: false, autoDelete: false);
        }

        public async Task ExecuteAsync<T>(CancellationToken stoppingToken) where T : class
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.Received += async (ch, ea) =>
            {
                try
                {
                    var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var @event = JsonConvert.DeserializeObject<T>(message);

                    // Process the event: fetch data from repository
                    var responseData = await _mediator.Send(@event!, stoppingToken);

                    // Prepare response
                    var replyProps = ea.BasicProperties;
                    //var replyTo = replyProps.ReplyTo; // the reply queue
                    replyProps.ReplyTo = _settings.EventQueue;
                    replyProps.CorrelationId = ea.BasicProperties.CorrelationId;

                    if (!string.IsNullOrEmpty(_settings.EventQueue))
                    {
                        var responseJson = JsonConvert.SerializeObject(responseData);
                        var body = Encoding.UTF8.GetBytes(responseJson);

                        var basicProperties = _channel?.CreateBasicProperties();
                        basicProperties!.CorrelationId = replyProps.CorrelationId;

                        // If the message expects a reply (based on ReplyTo property),
                        // you send the reply by calling BasicPublish() inside the handler.
                        // You publish the reply to the exchange / queue specified in ea.BasicProperties.ReplyTo.
                        _channel?.BasicPublish(exchange: _settings.Fanout, routingKey: "", basicProperties: basicProperties, body: body);
                    }

                    _channel?.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    // NACK the message
                    _channel?.BasicNack(ea.DeliveryTag, multiple: false, requeue: true);
                    // Discard the message:
                    // _channel?.BasicReject(ea.DeliveryTag, requeue: false);
                }

                // ToDo: Ensure you handle message retries and dead-lettering if messages repeatedly fail.
            };

            _channel.BasicConsume(_settings.EventQueue, autoAck: false, consumer);

            await Task.CompletedTask; // Keep the consumer alive
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection.Dispose();
        }
    }
}