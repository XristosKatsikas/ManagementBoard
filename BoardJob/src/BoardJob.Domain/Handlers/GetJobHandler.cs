using BoardJob.Domain.DTOs.Responses;
using BoardJob.Domain.Mappers;
using BoardJob.Domain.Repositories.Abstractions;
using MediatR;
using BoardJob.Domain.Handlers.Validators;
using Microsoft.Extensions.Logging;
using BoardJob.Core;
using FluentResults;
using BoardJob.Domain.Queries;

namespace BoardJob.Domain.Handlers
{
    public record GetJobHandler : IRequestHandler<GetJobQuery, IResult<JobResponse>>
    {
        private readonly ILogger<GetJobHandler> _logger;
        private readonly IJobRepository _jobRepository;

        public GetJobHandler(IJobRepository jobRepository, ILogger<GetJobHandler> logger)
        {
            _jobRepository = jobRepository;
            _logger = logger;
        }

        public async Task<IResult<JobResponse>> Handle(GetJobQuery query, CancellationToken cancellationToken)
        {
            var validator = new GetJobQueryValidator();
            var validationResult = await validator.ValidateAsync(query, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.Errors.Select(val => val.ErrorMessage).ToList();
                _logger.LogError($"Validation errors occurred in GetJobHandler.{nameof(Handle)}: " +
                    $"{string.Join(", ", errorMessages)}");

                return Result.Fail<JobResponse>(FailedResultMessage.RequestValidation);
            }

            var entity = query.ToEntity();
            try
            {
                var result = await _jobRepository.GetAsyncByJobId(entity.Id);

                if (result is null)
                {
                    _logger.LogInformation($"No data to fetch from GetJobHandler.{nameof(Handle)}");
                    return Result.Fail<JobResponse>(FailedResultMessage.NotFound);
                }
                return Result.Ok(result!.ToResponse());
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetJobHandler.{nameof(Handle)} has failed with exception message: {ex.Message}");
                return Result.Fail<JobResponse>(FailedResultMessage.Exception);
            }
        }
    }
}