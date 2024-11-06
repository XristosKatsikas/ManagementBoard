using AutoMapper;
using BoardProject.Domain.DTOs.Requests.Project;
using BoardProject.Domain.DTOs.Responses;
using BoardProject.Domain.Entities;
using FluentResults;

namespace BoardProject.Domain.Mappers
{
    public static class ProjectMapper
    {
        private static IMapper Mapper { get; }

        static ProjectMapper()
        {
            Mapper = new MapperConfiguration(config => config.AddProfile<ProjectProfile>()).CreateMapper();
        }

        public static Project ToEntity(this AddProjectRequest request)
        {
            return Mapper.Map<Project>(request);
        }

        public static Project ToEntity(this EditProjectRequest request)
        {
            return Mapper.Map<Project>(request);
        }

        public static Project ToEntity(this GetProjectRequest request)
        {
            return Mapper.Map<Project>(request);
        }

        public static Project ToEntity(this DeleteProjectRequest request)
        {
            return Mapper.Map<Project>(request);
        }

        public static ProjectResponse ToResponse(this Project project)
        {
            return Mapper.Map<ProjectResponse>(project);
        }

        public static IEnumerable<ProjectResponse> ToEnumerableResponse(this IEnumerable<Project> project)
        {
            return Mapper.Map<IEnumerable<ProjectResponse>>(project);
        }
    }
}
