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
        private readonly ILogger<GetJobsByProjectIdEventBackgroundService> _logger;

        private readonly RmqConsumer _consumer;

        public GetJobsByProjectIdEventBackgroundService(
            IMediator mediator,
            EventBusSettings settings,
            ConnectionFactory factory,
            ILogger<GetJobsByProjectIdEventBackgroundService> logger)
        {
            _logger = logger;
            _consumer = new RmqConsumer(mediator, settings, factory);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await _consumer.ConsumeAsync<GetJobsByProjectIdEvent>(stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogError($"Unable to consume the event bus: {e.Message}");
                throw;
            }
        }
    }
}
