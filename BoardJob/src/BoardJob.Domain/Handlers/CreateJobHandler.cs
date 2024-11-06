using BoardJob.Core;
using BoardJob.Domain.Commands;
using BoardJob.Domain.DTOs.Responses;
using BoardJob.Domain.Handlers.Validators;
using BoardJob.Domain.Mappers;
using BoardJob.Domain.Repositories.Abstraction;
using BoardJob.Domain.Repositories.Abstractions;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BoardJob.Domain.Handlers
{
    public record CreateJobHandler : IRequestHandler<CreateJobCommand, IResult<JobResponse>>
    {
        private readonly IJobRepository _jobRepository;
        private readonly ILogger<CreateJobHandler> _logger;

        public CreateJobHandler(IJobRepository jobRepository, ILogger<CreateJobHandler> logger)
        {
            _jobRepository = jobRepository;
            _logger = logger;
        }

        public async Task<IResult<JobResponse>> Handle(CreateJobCommand command, CancellationToken cancellationToken)
        {
            var validator = new CreateJobCommandValidator();
            var validationResult = await validator.ValidateAsync(command, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.Errors.Select(val => val.ErrorMessage).ToList();
                _logger.LogError($"Validation errors occurred in CreateJobHandler.{nameof(Handle)}: " +
                    $"{string.Join(", ", errorMessages)}");

                return Result.Fail<JobResponse>(FailedResultMessage.RequestValidation);
            }

            var entity = command.ToEntity();
            try
            {
                var result = _jobRepository.AddJob(entity);
                if (result is null)
                {
                    _logger.LogError($"Post data from CreateJobHandler.{nameof(Handle)} has failed.");
                    return Result.Fail<JobResponse>(FailedResultMessage.Unprocessable);
                }

                await _jobRepository.UnitOfWork.SaveChangesAsync();

                return Result.Ok(result.ToResponse());
            }
            catch (Exception ex)
            {
                _logger.LogError($"CreateJobHandler.{nameof(Handle)} has failed with exception message: {ex.Message}");
                return Result.Fail<JobResponse>(FailedResultMessage.Exception);
            }
        }
    }
}
