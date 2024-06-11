using BoardProject.Domain.Entities;
using BoardProject.Domain.Repositories.Abstraction;

namespace BoardProject.Domain.Repositories.Abstractions
{
    public interface IProjectRepository
    {
        Task<IEnumerable<Project>> GetAllProjectsAsync();
        Task<Project?> GetAsyncByProjectId(Guid id);
        Project AddProject(Project project);
        Project UpdateProject(Project project);
        bool DeleteProject(Project project);
        IUnitOfWork UnitOfWork { get; }
    }
}
