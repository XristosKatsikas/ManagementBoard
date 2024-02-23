using BoardJob.Domain.DTOs.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardJob.Domain.Commands
{
    public class DeleteJobCommand : IRequest<JobResponse>
    {
        public Guid Id { get; set; }
    }
}
