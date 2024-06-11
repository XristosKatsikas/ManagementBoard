using BoardJob.Domain.DTOs.Responses;
using BoardJob.Domain.Mappers;
using BoardJob.Domain.Repositories.Abstractions;
using MediatR;
using BoardJob.Domain.Commands;
using BoardJob.Domain.Handlers.Validators;
using Microsoft.Extensions.Logging;

namespace BoardJob.Domain.Handlers
{
    public record EditJobHandler : IRequestHandler<EditJobCommand, JobResponse>
    {
        private readonly ILogger<EditJobHandler> _logger;
        private readonly IJobRepository _jobRepository;

        public EditJobHandler(IJobRepository jobRepository, ILogger<EditJobHandler> logger)
        {
            _jobRepository = jobRepository;
            _logger = logger;
        }

        public async Task<JobResponse> Handle(EditJobCommand command, CancellationToken cancellationToken)
        {
            var validator = new EditJobCommandValidator();
            var validationResult = await validator.ValidateAsync(command, cancellationToken);

            if (!validationResult.IsValid)
            {
                _logger.LogError(string.Format("Validation of command with Id {0} has failed", command.Id));
                return null!;
            }

            var entity = command.ToEntity();
            var result = _jobRepository.UpdateJob(entity);
            await _jobRepository.UnitOfWork.SaveChangesAsync();

            var jobResponse = result.ToResponse();

            return jobResponse;
        }
    }
}
