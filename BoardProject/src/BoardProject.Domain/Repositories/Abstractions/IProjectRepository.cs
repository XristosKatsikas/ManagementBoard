using BoardProject.Domain.Entities;
using BoardProject.Domain.Repositories.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardProject.Domain.Repositories.Abstractions
{
    public interface IProjectRepository
    {
        Task<IEnumerable<Project>> GetAllProjectsAsync();
        Task<Project?> GetAsyncByProjectId(Guid id);
        Project AddProject(Project project);
        Project UpdateProject(Project project);
        Project DeleteProject(Project project);
        IUnitOfWork UnitOfWork { get; }
    }
}
