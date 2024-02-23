using BoardProject.Domain.Entities;
using BoardProject.Domain.Enums;

namespace BoardProject.Domain.DTOs.Responses
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
        public Project ParentProject { get; set; } = null!;
        public Guid ProjectId { get; set; }
    }
}
