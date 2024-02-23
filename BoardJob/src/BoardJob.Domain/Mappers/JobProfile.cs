using AutoMapper;
using BoardJob.Domain.Commands;
using BoardJob.Domain.DTOs.Responses;
using BoardJob.Domain.Entities;

namespace BoardJob.Domain.Mappers
{
    public class JobProfile : Profile
    {
        public JobProfile()
        {
            CreateMap<CreateJobCommand, Job>()
                .ForMember(dst => dst.Progress, opt => opt.MapFrom(src => src.Progress))
                .ForMember(dst => dst.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dst => dst.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dst => dst.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dst => dst.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dst => dst.FinishDate, opt => opt.MapFrom(src => src.FinishDate))
                .ForMember(dst => dst.ProjectId, opt => opt.MapFrom(src => src.ProjectId))
                .ReverseMap();

            CreateMap<EditJobCommand, Job>()
                .ForMember(dst => dst.Progress, opt => opt.MapFrom(src => src.Progress))
                .ForMember(dst => dst.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dst => dst.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dst => dst.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dst => dst.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dst => dst.FinishDate, opt => opt.MapFrom(src => src.FinishDate))
                .ForMember(dst => dst.ProjectId, opt => opt.MapFrom(src => src.ProjectId))
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ReverseMap();

            CreateMap<GetJobCommand, Job>()
               .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
               .ReverseMap();

            CreateMap<DeleteJobCommand, Job>()
               .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
               .ReverseMap();

            CreateMap<Job, JobResponse>()
                .ForMember(dst => dst.Progress, opt => opt.MapFrom(src => src.Progress))
                .ForMember(dst => dst.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dst => dst.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dst => dst.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dst => dst.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dst => dst.FinishDate, opt => opt.MapFrom(src => src.FinishDate))
                .ForMember(dst => dst.ProjectId, opt => opt.MapFrom(src => src.ProjectId))
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ReverseMap();
        }
    }
}
