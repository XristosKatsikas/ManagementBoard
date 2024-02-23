using BoardProject.Domain.Entities;
using BoardProject.Domain.Repositories.Abstraction;
using BoardProject.Domain.Repositories.Abstractions;

namespace BoardProject.Infrastructure.Repositories
{
    public class ProjectRepository : GenericRepository<Project>, IProjectRepository
    {
        private readonly ApplicationDbContext _context;

        public IUnitOfWork UnitOfWork => _context;

        public ProjectRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public Project AddProject(Project project)
        {
            if (project is not null)
            {
                return Add(project);
            }
            return null!;
        }

        public Project DeleteProject(Project project)
        {
            if (project is not null)
            {
                return Delete(project);
            }
            return null!;
        }

        public async Task<IEnumerable<Project>> GetAllProjectsAsync()
        {
            return await GetAllAsync();
        }

        public async Task<Project?> GetAsyncByProjectId(Guid id)
        {
            return await GetAsyncById(id);
        }

        public Project UpdateProject(Project project)
        {
            if (project is not null)
            {
                return Update(project);
            }
            return null!;
        }
    }
}
