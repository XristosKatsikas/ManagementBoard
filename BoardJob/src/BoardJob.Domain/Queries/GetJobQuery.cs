using BoardJob.Domain.DTOs.Responses;
using FluentResults;
using MediatR;

namespace BoardJob.Domain.Queries
{
    public record GetJobQuery : IRequest<IResult<JobResponse>>
    {
        public Guid Id { get; set; }
    }
}
