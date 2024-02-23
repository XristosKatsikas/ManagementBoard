using BoardJob.Domain.DTOs.Responses;
using MediatR;

namespace BoardJob.Domain.Commands
{
    public class GetJobCommand : IRequest<JobResponse>
    {
        public Guid Id { get; set; }
    }
}
