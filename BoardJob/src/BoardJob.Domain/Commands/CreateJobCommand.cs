using BoardJob.Domain.DTOs.Responses;
using BoardJob.Domain.Enums;
using MediatR;

namespace BoardJob.Domain.Commands
{
    public record CreateJobCommand : IRequest<JobResponse>
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
