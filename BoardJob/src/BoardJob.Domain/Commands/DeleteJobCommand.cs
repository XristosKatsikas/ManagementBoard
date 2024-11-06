using BoardJob.Domain.DTOs.Responses;
using FluentResults;
using MediatR;

namespace BoardJob.Domain.Commands
{
    public record DeleteJobCommand : IRequest<IResult<bool>>
    {
        public Guid Id { get; set; }
    }
}