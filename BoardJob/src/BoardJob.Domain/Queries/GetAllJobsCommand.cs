using BoardJob.Domain.DTOs.Responses;
using FluentResults;
using MediatR;

namespace BoardJob.Domain.Queries
{
    public record GetAllJobsCommand : IRequest<IResult<IEnumerable<JobResponse>>>
    {
    }
}