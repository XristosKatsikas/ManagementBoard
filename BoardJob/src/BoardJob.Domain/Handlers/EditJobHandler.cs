using BoardJob.Domain.DTOs.Responses;
using BoardJob.Domain.Mappers;
using BoardJob.Domain.Repositories.Abstractions;
using MediatR;
using BoardJob.Domain.Commands;
using BoardJob.Domain.Handlers.Validators;
using Microsoft.Extensions.Logging;
using FluentResults;
using BoardJob.Core;

namespace BoardJob.Domain.Handlers
{
    public record EditJobHandler : IRequestHandler<EditJobCommand, IResult<JobResponse>>
    {
        private readonly ILogger<EditJobHandler> _logger;
        private readonly IJobRepository _jobRepository;

        public EditJobHandler(IJobRepository jobRepository, ILogger<EditJobHandler> logger)
        {
            _jobRepository = jobRepository;
            _logger = logger;
        }

        public async Task<IResult<JobResponse>> Handle(EditJobCommand command, CancellationToken cancellationToken)
        {
            var validator = new EditJobCommandValidator();
            var validationResult = await validator.ValidateAsync(command, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.Errors.Select(val => val.ErrorMessage).ToList();
                _logger.LogError($"Validation errors occurred in EditJobHandler.{nameof(Handle)}: " +
                    $"{string.Join(", ", errorMessages)}");

                return Result.Fail<JobResponse>(FailedResultMessage.RequestValidation);
            }

            var entity = command.ToEntity();
            try
            {
                var result = _jobRepository.UpdateJob(entity);
                if (result is null)
                {
                    _logger.LogError($"Update data from EditJobHandler.{nameof(Handle)} has failed.");
                    return Result.Fail<JobResponse>(FailedResultMessage.Unprocessable);
                }
                await _jobRepository.UnitOfWork.SaveChangesAsync();

                var jobResponse = result.ToResponse();

                return Result.Ok(jobResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"EditJobHandler.{nameof(Handle)} has failed with exception message: {ex.Message}");
                return Result.Fail<JobResponse>(FailedResultMessage.Exception);
            }
        }
    }
}
