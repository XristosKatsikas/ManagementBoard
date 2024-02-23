using BoardJob.Domain.Entities;
using BoardJob.Domain.Enums;

namespace BoardJob.Domain.DTOs.Responses
{
    public record JobResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Status Status { get; set; }
        public decimal Progress { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public Guid ProjectId { get; set; }
    }
}
