using AutoMapper;
using BoardProject.Domain.DTOs.Requests.Project;
using BoardProject.Domain.DTOs.Responses;
using BoardProject.Domain.Entities;

namespace BoardProject.Domain.Mappers
{
    public class ProjectProfile : Profile
    {
        public ProjectProfile()
        {
            CreateMap<AddProjectRequest, Project>()
                .ForMember(dst => dst.Progress, opt => opt.MapFrom(src => src.Progress))
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dst => dst.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dst => dst.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dst => dst.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dst => dst.FinishDate, opt => opt.MapFrom(src => src.FinishDate))
                .ReverseMap();

            CreateMap<EditProjectRequest, Project>()
                .ForMember(dst => dst.Progress, opt => opt.MapFrom(src => src.Progress))
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dst => dst.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dst => dst.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dst => dst.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dst => dst.FinishDate, opt => opt.MapFrom(src => src.FinishDate))
                .ReverseMap();

            CreateMap<GetProjectRequest, Project>()
                .ForMember(dst => dst.ProjectId, opt => opt.MapFrom(src => src.Id))
                .ReverseMap();

            CreateMap<DeleteProjectRequest, Project>()
                .ForMember(dst => dst.ProjectId, opt => opt.MapFrom(src => src.Id))
                .ReverseMap();

            CreateMap<Project, ProjectResponse>()
                .ForMember(dst => dst.ProjectId, opt => opt.MapFrom(src => src.ProjectId))
                .ForMember(dst => dst.Progress, opt => opt.MapFrom(src => src.Progress))
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dst => dst.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dst => dst.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dst => dst.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dst => dst.FinishDate, opt => opt.MapFrom(src => src.FinishDate))
                .ReverseMap();
        }
    }
}
