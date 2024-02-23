using AutoMapper;
using BoardJob.Domain.Commands;
using BoardJob.Domain.DTOs.Responses;
using BoardJob.Domain.Entities;

namespace BoardJob.Domain.Mappers
{
    public static class JobMapper
    {
        private static IMapper Mapper { get; }

        static JobMapper()
        {
            Mapper = new MapperConfiguration(config => config.AddProfile<JobProfile>()).CreateMapper();
        }

        public static Job ToEntity(this CreateJobCommand command)
        {
            return Mapper.Map<Job>(command);
        }

        public static Job ToEntity(this EditJobCommand command)
        {
            return Mapper.Map<Job>(command);
        }

        public static Job ToEntity(this GetJobCommand command)
        {
            return Mapper.Map<Job>(command);
        }

        public static Job ToEntity(this DeleteJobCommand command)
        {
            return Mapper.Map<Job>(command);
        }

        public static JobResponse ToResponse(this Job job)
        {
            return Mapper.Map<JobResponse>(job);
        }

        public static IEnumerable<JobResponse> ToEnumerableResponse(this IEnumerable<Job> job)
        {
            return Mapper.Map<IEnumerable<JobResponse>>(job);
        }

        public static JobResponse EnumerableToResponse(this IEnumerable<Job> job)
        {
            return Mapper.Map<JobResponse>(job);
        }
    }
}
