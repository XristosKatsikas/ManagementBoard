using BoardJob.Domain.DTOs.Responses;
using MediatR;

namespace BoardJob.Domain.Commands
{
    public record GetJobCommand : IRequest<JobResponse>
    {
        public Guid Id { get; set; }
    }
}
