using BoardJob.Domain.DTOs.Responses;
using BoardJob.Domain.Enums;
using BoardJob.Domain.Events.Job;
using BoardJob.Domain.Repositories.Abstractions;
using MediatR;

namespace BoardJob.Domain.Handlers
{
    public class GetJobsByProjectIdEventHandler : IRequestHandler<GetJobsByProjectIdEvent, IEnumerable<JobResponse>>
    {
        private readonly IJobRepository _jobRepository;
        public GetJobsByProjectIdEventHandler(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }

        public async Task<IEnumerable<JobResponse>> Handle(GetJobsByProjectIdEvent @event, CancellationToken cancellationToken)
        {
            var jobs = await _jobRepository.GetAllJobsByProjectIdAsync(@event.ProjectId);
            return jobs.Select(job => new JobResponse
            {
                Title = job.Title,
                Description = job.Description,
                Status = job.Status,
                Progress = job.Progress,
                StartDate = job.StartDate,
                FinishDate = job.FinishDate, 
                ProjectId = job.ProjectId
            }).ToList();
        }
    }
}