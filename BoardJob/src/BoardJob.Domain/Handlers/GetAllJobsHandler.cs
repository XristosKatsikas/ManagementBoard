using BoardJob.Core;
using BoardJob.Domain.DTOs.Responses;
using BoardJob.Domain.Mappers;
using BoardJob.Domain.Queries;
using BoardJob.Domain.Repositories.Abstractions;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;


namespace BoardJob.Domain.Handlers
{
    public record GetAllJobsHandler : IRequestHandler<GetAllJobsCommand, IResult<IEnumerable<JobResponse>>>
    {
        private readonly ILogger<GetAllJobsHandler> _logger;
        private readonly IJobRepository _jobRepository;

        public GetAllJobsHandler(
            ILogger<GetAllJobsHandler> logger,
            IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
            _logger = logger;
        }

        public async Task<IResult<IEnumerable<JobResponse>>> Handle(GetAllJobsCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var getJobs = await _jobRepository.GetAllJobsAsync();
                var totalJobs = getJobs.Count();

                if (totalJobs == 0)
                {
                    _logger.LogInformation($"No data to fetch in GetAllJobsHandler.{nameof(Handle)}");
                    return Result.Fail<IEnumerable<JobResponse>>(FailedResultMessage.NotFound);
                }

                return Result.Ok(getJobs.EnumerableToResponse());
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetAllJobsHandler.{nameof(Handle)} has failed with exception message: {ex.Message}");
                return Result.Fail<IEnumerable<JobResponse>>(FailedResultMessage.Exception);
            }
        }
    }
}
