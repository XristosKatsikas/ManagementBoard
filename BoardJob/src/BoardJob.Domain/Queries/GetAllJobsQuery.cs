using BoardJob.Domain.DTOs.Responses;
using FluentResults;
using MediatR;

namespace BoardJob.Domain.Queries
{
    public record GetAllJobsQuery : IRequest<IResult<IEnumerable<JobResponse>>>
    {
    }
}