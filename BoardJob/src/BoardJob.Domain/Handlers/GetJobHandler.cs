using BoardJob.Domain.DTOs.Responses;
using BoardJob.Domain.Mappers;
using BoardJob.Domain.Repositories.Abstractions;
using MediatR;
using BoardJob.Domain.Commands;
using BoardJob.Domain.Handlers.Validators;
using Microsoft.Extensions.Logging;

namespace BoardJob.Domain.Handlers
{
    public record GetJobHandler : IRequestHandler<GetJobCommand, JobResponse>
    {
        private readonly ILogger<GetJobHandler> _logger;
        private readonly IJobRepository _jobRepository;

        public GetJobHandler(IJobRepository jobRepository, ILogger<GetJobHandler> logger)
        {
            _jobRepository = jobRepository;
            _logger = logger;
        }

        public async Task<JobResponse> Handle(GetJobCommand command, CancellationToken cancellationToken)
        {
            var validator = new GetJobCommandValidator();
            var validationResult = await validator.ValidateAsync(command, cancellationToken);

            if (!validationResult.IsValid)
            {
                _logger.LogError(string.Format("Validation of command with Id {0} has failed", command.Id));
                return null!;
            }

            var entity = command.ToEntity();
            var result = await _jobRepository.GetAsyncByJobId(entity.Id);
            return result!.ToResponse();
        }
    }
}
