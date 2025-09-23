using BoardJob.Domain.Configurations;
using BoardJob.Domain.Events.Job;
using BoardJob.Infrastructure.RabbitMq;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace BoardJob.Infrastructure.BackgroundServices
{
    public class GetJobsByProjectIdEventBackgroundService : BackgroundService
    {
        private readonly IMediator _mediator;
        private readonly ILogger<GetJobsByProjectIdEventBackgroundService> _logger;
        private readonly EventBusSettings _settings;
        private readonly IModel? _channel;

        private readonly RmqConsumer _consumer;

        public GetJobsByProjectIdEventBackgroundService(
            IMediator mediator,
            EventBusSettings settings,
            ConnectionFactory factory,
            ILogger<GetJobsByProjectIdEventBackgroundService> logger)
        {
            _settings = settings;
            _logger = logger;
            _mediator = mediator;

            _consumer = new RmqConsumer(mediator, settings, factory);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await _consumer.ExecuteAsync<GetJobsByProjectIdEvent>(stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogError("Unable to consume the event bus: {message}", e.Message);
                throw;
            }
        }
    }
}
