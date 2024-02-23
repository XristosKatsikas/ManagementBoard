using BoardJob.Domain.Entities;
using BoardJob.Domain.Repositories.Abstraction;
using BoardJob.Domain.Repositories.Abstractions;

namespace BoardJob.Infrastructure.Repositories
{
    public class JobRepository : GenericRepository<Job>, IJobRepository
    {
        private readonly ApplicationDbContext _context;

        public IUnitOfWork UnitOfWork => _context;

        public JobRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Job>> GetAllJobsAsync()
        {
            return await GetAllAsync();
        }

        public async Task<Job?> GetAsyncByJobId(Guid id)
        {
            return await GetAsyncById(id);
        }

        public Job AddJob(Job job)
        {
            if (job is not null)
            {
                return Add(job);
            }
            return null!;
        }

        public Job UpdateJob(Job job)
        {
            if (job is not null)
            {
                return Update(job);
            }
            return null!;
        }

        public Job DeleteJob(Job job)
        {
            if (job is not null)
            {
                return Delete(job);
            }
            return null!;
        }

        public async Task<IEnumerable<Job>> GetAllJobsByProjectIdAsync(Guid id)
        {
            var getAllJobs = await GetAllJobsAsync();
            var jobsList = new List<Job>();
            foreach (var job in getAllJobs) 
            {
                if (job.ProjectId == id)
                {
                    jobsList.Add(job);
                }
            }
            return jobsList;
        }
    }
}
