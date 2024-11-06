using BoardJob.Domain.DTOs.Responses;
using FluentResults;
using MediatR;

namespace BoardJob.Domain.Commands
{
    public record GetJobCommand : IRequest<IResult<JobResponse>>
    {
        public Guid Id { get; set; }
    }
}
