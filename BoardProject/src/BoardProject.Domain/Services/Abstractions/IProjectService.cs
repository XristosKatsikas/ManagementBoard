using FluentResults;
using BoardProject.Domain.DTOs.Requests.Project;
using BoardProject.Domain.DTOs.Responses;


namespace BoardProject.Domain.Services.Abstractions
{
    public interface IProjectService
    {
        Task<IResult<IEnumerable<ProjectResponse>>> GetProjectsAsync(int pageSize, int pageIndex);
        Task<IResult<ProjectResponse>> GetProjectAsync(GetProjectRequest request);
        Task<IResult<ProjectResponse>> AddProjectAsync(AddProjectRequest request);
        Task<IResult<ProjectResponse>> EditProjectAsync(EditProjectRequest request);
        Task<IResult<bool>> DeleteProjectAsync(DeleteProjectRequest request);
    }
}
