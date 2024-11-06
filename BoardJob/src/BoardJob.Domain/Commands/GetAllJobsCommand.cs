using BoardJob.Domain.DTOs.Responses;
using FluentResults;
using MediatR;

namespace BoardJob.Domain.Commands
{
    public record GetAllJobsCommand : IRequest<IResult<IEnumerable<JobResponse>>>
    {
    }
}