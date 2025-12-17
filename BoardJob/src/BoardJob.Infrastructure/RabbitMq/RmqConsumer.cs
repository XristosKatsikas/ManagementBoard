using BoardJob.Domain.Configurations;
using MediatR;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace BoardJob.Infrastructure.RabbitMq
{
    public class RmqConsumer : IRmqConsumer, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IMediator _mediator;
        private readonly EventBusSettings _settings;
        private readonly IModel _channel;

        public RmqConsumer(IMediator mediator, EventBusSettings settings, ConnectionFactory factory)
        {
            _mediator = mediator;
            _settings = settings;
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            DeclareQueueAndExchange();
        }

        private void DeclareQueueAndExchange()
        {
            // Declare exchange if necessary
            //_channel.ExchangeDeclare(_settings.Fanout, ExchangeType.Fanout, durable: true);

            var args = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", "your-dead-letter-exchange" }
            };

            _channel.QueueDeclare(queue: _settings.EventQueue, durable: true, exclusive: false, autoDelete: false, arguments: args);
            // Optionally bind queue to exchange if needed
            // _channel.QueueBind(_settings.EventQueue, _settings.Fanout, routingKey: "");
        }

        public async Task ConsumeAsync<T>(CancellationToken stoppingToken) where T : class
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.Received += async (ch, ea) =>
            {
                try
                {
                    var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var @event = JsonConvert.DeserializeObject<T>(message);

                    // Call mediator to process event
                    var responseData = await _mediator.Send(@event!, stoppingToken);

                    // Prepare response
                    var replyProps = ea.BasicProperties;
                    replyProps.ReplyTo = _settings.EventQueue; // set reply queue if needed
                    replyProps.CorrelationId = ea.BasicProperties.CorrelationId;

                    if (!string.IsNullOrEmpty(replyProps.ReplyTo))
                    {
                        var responseJson = JsonConvert.SerializeObject(responseData);
                        var body = Encoding.UTF8.GetBytes(responseJson);
                        var basicProperties = _channel.CreateBasicProperties();
                        basicProperties.CorrelationId = replyProps.CorrelationId;

                        _channel.BasicPublish(exchange: _settings.Fanout, routingKey: "", basicProperties: basicProperties, body: body);
                    }

                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    // Log exception
                    // Optionally, send message to dead-letter or requeue
                    _channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: false);
                    // Consider logging ex
                }
            };

            _channel.BasicConsume(queue: _settings.EventQueue, autoAck: false, consumer: consumer);

            await Task.CompletedTask; // Keep alive
        }

        public void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}