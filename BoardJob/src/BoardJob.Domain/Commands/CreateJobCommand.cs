using BoardJob.Domain.DTOs.Responses;
using BoardJob.Domain.Enums;
using FluentResults;
using MediatR;

namespace BoardJob.Domain.Commands
{
    public record CreateJobCommand : IRequest<IResult<JobResponse>>
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Status Status { get; set; }
        public decimal Progress { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public Guid ProjectId { get; set; }
    }
}
