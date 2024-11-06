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
    public record DeleteJobHandler : IRequestHandler<DeleteJobCommand, IResult<bool>>
    {
        private readonly ILogger<DeleteJobHandler> _logger;
        private readonly IJobRepository _jobRepository;

        public DeleteJobHandler(IJobRepository jobRepository, ILogger<DeleteJobHandler> logger)
        {
            _jobRepository = jobRepository;
            _logger = logger;
        }

        public async Task<IResult<bool>> Handle(DeleteJobCommand command, CancellationToken cancellationToken)
        {
            var validator = new DeleteJobCommandValidator();
            var validationResult = await validator.ValidateAsync(command, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.Errors.Select(val => val.ErrorMessage).ToList();
                _logger.LogError($"Validation errors occurred in DeleteJobHandler.{nameof(Handle)}: " +
                    $"{string.Join(", ", errorMessages)}");

                return Result.Fail<bool>(FailedResultMessage.RequestValidation);
            }

            var entity = command.ToEntity();
            try
            {
                var isJobDeleted = _jobRepository.DeleteJob(entity);

                if (!isJobDeleted)
                {
                    _logger.LogError($"Delete data from DeleteJobHandler.{nameof(Handle)} has failed.");
                    return Result.Fail<bool>(FailedResultMessage.NotFound);
                }

                await _jobRepository.UnitOfWork.SaveChangesAsync();

                return Result.Ok(isJobDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError($"DeleteobHandler.{nameof(Handle)} has failed with exception message: {ex.Message}");
                return Result.Fail<bool>(FailedResultMessage.Exception);
            }
        }
    }
}
