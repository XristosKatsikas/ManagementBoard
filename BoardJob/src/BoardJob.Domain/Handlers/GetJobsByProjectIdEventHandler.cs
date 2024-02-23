using BoardJob.Domain.Events.Job;
using BoardJob.Domain.Repositories.Abstractions;
using MediatR;

namespace BoardJob.Domain.Handlers
{
    public record GetJobsByProjectIdEventHandler : IRequestHandler<GetJobsByProjectIdEvent, Unit>
    {
        private readonly IJobRepository _jobRepository;
        public GetJobsByProjectIdEventHandler(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }

        public async Task<Unit> Handle(GetJobsByProjectIdEvent @event, CancellationToken cancellationToken)
        {
            var jobs = await _jobRepository.GetAllJobsAsync();
            var jobIds = jobs.ToList();
            var jobsByProjectId = jobIds.Select(async x =>
            {
                await _jobRepository.GetAllJobsByProjectIdAsync(@event.ProjectId);
            });

            await Task.WhenAll(jobsByProjectId);
            return Unit.Value;
        }
    }
}
