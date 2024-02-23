using BoardJob.Domain.DTOs.Responses;
using BoardJob.Domain.Mappers;
using BoardJob.Domain.Repositories.Abstractions;
using MediatR;
using BoardJob.Domain.Commands;
using BoardJob.Domain.Handlers.Validators;
using Microsoft.Extensions.Logging;

namespace BoardJob.Domain.Handlers
{
    public record DeleteJobHandler : IRequestHandler<DeleteJobCommand, JobResponse>
    {
        private readonly ILogger<DeleteJobHandler> _logger;
        private readonly IJobRepository _jobRepository;

        public DeleteJobHandler(IJobRepository jobRepository, ILogger<DeleteJobHandler> logger)
        {
            _jobRepository = jobRepository;
            _logger = logger;
        }

        public async Task<JobResponse> Handle(DeleteJobCommand command, CancellationToken cancellationToken)
        {
            var validator = new DeleteJobCommandValidator();
            var validationResult = await validator.ValidateAsync(command);

            if (!validationResult.IsValid)
            {
                _logger.LogError(string.Format("Validation of command with Id {0} has failed", command.Id));
                return null!;
            }

            var entity = command.ToEntity();
            var result = _jobRepository.DeleteJob(entity);

            await _jobRepository.UnitOfWork.SaveChangesAsync();

            return result!.ToResponse();
        }
    }
}
