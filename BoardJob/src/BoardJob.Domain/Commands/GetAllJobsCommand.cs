using BoardJob.Domain.DTOs.Responses;
using MediatR;


namespace BoardJob.Domain.Commands
{
    public class GetAllJobsCommand : IRequest<JobResponse>
    {
    }
}
