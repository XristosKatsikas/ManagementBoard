using BoardJob.Domain.Configurations;
using BoardJob.Domain.Events.Job;
using BoardJob.Infrastructure.RabbitMq;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace BoardJob.Infrastructure.BackgroundServices
{
    public class GetJobsByProjectIdEventBackgroundService : BackgroundService
    {
        private readonly IMediator _mediator;
        private readonly ILogger<GetJobsByProjectIdEventBackgroundService> _logger;
        private readonly EventBusSettings _settings;
        private readonly IModel? _channel;

        private readonly Consumer _consumer;

        public GetJobsByProjectIdEventBackgroundService(
            IMediator mediator,
            EventBusSettings settings,
            ConnectionFactory factory,
            ILogger<GetJobsByProjectIdEventBackgroundService> logger)
        {
            _settings = settings;
            _logger = logger;
            _mediator = mediator;

            _consumer = new Consumer(mediator, settings, factory);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _consumer.ExecuteAsync<GetJobsByProjectIdEvent>(stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogWarning("Unable to consume the event bus: {message}", e.Message);
            }

            return Task.CompletedTask;
        }
    }
}
