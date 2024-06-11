using BoardJob.Domain.DTOs.Responses;
using MediatR;

namespace BoardJob.Domain.Commands
{
    public record DeleteJobCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }
}
