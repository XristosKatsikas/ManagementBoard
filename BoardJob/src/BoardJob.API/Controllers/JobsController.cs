using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BoardJob.Domain.Commands;
using MediatR;
using BoardJob.Domain.Events.Job;
using BoardJob.API;

namespace BoardJob.Api.Controllers
{
    [ApiController]
    [Route("/[controller]/")]
    [Authorize]
    public class JobsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public JobsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id:guid}")]
        [ApiConventionMethod(typeof(ApiConvention), nameof(DefaultApiConventions.Get))]
        public async Task<IActionResult> GetJobById(Guid id)
        {
            var result = await _mediator.Send(new GetJobCommand { Id = id });
            return Ok(result);
        }

        [HttpPost]
        [ApiConventionMethod(typeof(ApiConvention), nameof(DefaultApiConventions.Create))]
        public async Task<IActionResult> PostJobAsync(CreateJobCommand request)
        {
            var result = await _mediator.Send(request);
            return CreatedAtAction(nameof(GetJobById), new { id = result.Id }, null);
        }

        [HttpPut("{jobId:guid}")]
        [ApiConventionMethod(typeof(ApiConvention), nameof(DefaultApiConventions.Update))]
        public async Task<IActionResult> EditJobAsync(Guid jobId, EditJobCommand command)
        {
            command.Id = jobId;
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [HttpDelete("{jobId:guid}")]
        [ApiConventionMethod(typeof(ApiConvention), nameof(DefaultApiConventions.Delete))]
        public async Task<IActionResult> DeleteJobAsync(Guid jobId)
        {
            var result = await _mediator.Send(new DeleteJobCommand
            {
                Id = jobId
            });

            return Ok(result);
        }

        [HttpGet("")]
        [ApiConventionMethod(typeof(ApiConvention), nameof(DefaultApiConventions.Get))]
        public async Task<IActionResult> GetAllJobsAsync()
        {
            var result = await _mediator.Send(new GetAllJobsCommand());
            return Ok(result);
        }

        [HttpGet("{projectId:guid}/jobs")]
        [ApiConventionMethod(typeof(ApiConvention), nameof(DefaultApiConventions.Get))]
        public async Task<IActionResult> GetAllJobsByProjectIdAsync(Guid projectId)
        {
            var result = await _mediator.Send(new GetJobsByProjectIdEvent
            {
                ProjectId = projectId
            });
            return Ok(result);
        }
    }
}
