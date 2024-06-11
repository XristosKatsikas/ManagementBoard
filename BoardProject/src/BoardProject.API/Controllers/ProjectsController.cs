using BoardProject.Core;
using BoardProject.Domain.DTOs.Requests.Project;
using BoardProject.Domain.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BoardProject.API.Controllers
{
    [ApiController]
    [Route("/[controller]/")]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpPost()]
        [ApiConventionMethod(typeof(ApiConvention), nameof(DefaultApiConventions.Create))]
        public async Task<IActionResult> AddItemAsync([FromBody] AddProjectRequest addProjectRequest)
        {
            return new ObjectResult(await _projectService.AddProjectAsync(addProjectRequest));
        }

        [HttpPut("{id:guid}")]
        [ApiConventionMethod(typeof(ApiConvention), nameof(DefaultApiConventions.Update))]
        public async Task<IActionResult> EditItemAsync(Guid id, [FromBody] EditProjectRequest editProjectRequest)
        {
            editProjectRequest.Id = id;
            return new ObjectResult(await _projectService.EditProjectAsync(editProjectRequest));
        }

        [HttpDelete("{id:guid}")]
        [ApiConventionMethod(typeof(ApiConvention), nameof(DefaultApiConventions.Delete))]
        public async Task<IActionResult> DeleteItemAsync(Guid id, DeleteProjectRequest deleteProjectRequest)
        {
            deleteProjectRequest.Id = id;
            return new ObjectResult(await _projectService.DeleteProjectAsync(deleteProjectRequest));
        }

        [HttpGet("{id:guid}")]
        [ApiConventionMethod(typeof(ApiConvention), nameof(DefaultApiConventions.Get))]
        public async Task<IActionResult> GetItemAsync(Guid id, GetProjectRequest getProjectRequest)
        {
            getProjectRequest.Id = id;
            return new ObjectResult(await _projectService.GetProjectAsync(getProjectRequest));
        }

        [HttpGet("all")]
        [ApiConventionMethod(typeof(ApiConvention), nameof(DefaultApiConventions.Get))]
        public async Task<IActionResult> GetItemsAsync([FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0)
        {
            return new ObjectResult(await _projectService.GetProjectsAsync(pageSize, pageIndex));
        }
    }
}
