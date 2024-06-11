using BoardJob.Domain.Entities;
using BoardJob.Domain.Repositories.Abstraction;
namespace BoardJob.Domain.Repositories.Abstractions
{
    public interface IJobRepository
    {
        Task<IEnumerable<Job>> GetAllJobsAsync();
        Task<IEnumerable<Job>> GetAllJobsByProjectIdAsync(Guid id);
        Task<Job?> GetAsyncByJobId(Guid id);
        Job AddJob(Job job);
        Job UpdateJob(Job job);
        bool DeleteJob(Job job);
        IUnitOfWork UnitOfWork { get; }
    }
}
