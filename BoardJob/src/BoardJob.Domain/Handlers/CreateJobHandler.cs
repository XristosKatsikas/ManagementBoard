using BoardJob.Domain.Commands;
using BoardJob.Domain.DTOs.Responses;
using BoardJob.Domain.Handlers.Validators;
using BoardJob.Domain.Mappers;
using BoardJob.Domain.Repositories.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BoardJob.Domain.Handlers
{
    public record CreateJobHandler : IRequestHandler<CreateJobCommand, JobResponse>
    {
        private readonly IJobRepository _jobRepository;
        private readonly ILogger<CreateJobHandler> _logger;

        public CreateJobHandler(IJobRepository jobRepository, ILogger<CreateJobHandler> logger)
        {
            _jobRepository = jobRepository;
            _logger = logger;
        }

        public async Task<JobResponse> Handle(CreateJobCommand command, CancellationToken cancellationToken)
        {
            var validator = new CreateJobCommandValidator();
            var validationResult = await validator.ValidateAsync(command, cancellationToken);

            if (!validationResult.IsValid)
            {
                _logger.LogError(string.Format("Validation of command with projectId {0} has failed", command.ProjectId));
                return null!;
            }

            var entity = command.ToEntity();
            var result = _jobRepository.AddJob(entity);
            await _jobRepository.UnitOfWork.SaveChangesAsync();

            return result.ToResponse();
        }
    }
}
