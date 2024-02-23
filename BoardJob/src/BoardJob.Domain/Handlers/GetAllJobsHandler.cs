using BoardJob.Domain.Commands;
using BoardJob.Domain.DTOs.Responses;
using BoardJob.Domain.Mappers;
using BoardJob.Domain.Repositories.Abstractions;
using MediatR;


namespace BoardJob.Domain.Handlers
{
    public record GetAllJobsHandler : IRequestHandler<GetAllJobsCommand, JobResponse>
    {
        private readonly IJobRepository _jobRepository;

        public GetAllJobsHandler(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }

        public async Task<JobResponse> Handle(GetAllJobsCommand command, CancellationToken cancellationToken)
        {
            var result = await _jobRepository.GetAllJobsAsync();
            return result.EnumerableToResponse();
        }
    }
}
