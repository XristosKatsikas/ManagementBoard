using BoardJob.Domain.DTOs.Responses;
using MediatR;

namespace BoardJob.Domain.Commands
{
    public record GetAllJobsCommand : IRequest<JobResponse>
    {
    }
}
