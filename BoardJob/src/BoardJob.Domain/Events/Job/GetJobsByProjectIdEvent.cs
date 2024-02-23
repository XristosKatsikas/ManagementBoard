using MediatR;

namespace BoardJob.Domain.Events.Job
{
    public class GetJobsByProjectIdEvent : IRequest<Unit>
    {
        public Guid ProjectId { get; set; }
    }
}
