using BoardJob.Domain.DTOs.Responses;
using MediatR;

namespace BoardJob.Domain.Events.Job
{
    public class GetJobsByProjectIdEvent : IRequest<IEnumerable<JobResponse>>
    {
        public Guid ProjectId { get; set; }
    }
}
