using BoardProject.Core;
using BoardProject.Domain.Configurations;
using BoardProject.Domain.DTOs.Requests.Project;
using BoardProject.Domain.DTOs.Requests.Project.Validators;
using BoardProject.Domain.DTOs.Responses;
using BoardProject.Domain.Events.Project;
using BoardProject.Domain.Mappers;
using BoardProject.Domain.Repositories.Abstractions;
using BoardProject.Domain.Services.Abstractions;
using BoardProject.Domain.Services.RabbitMq;
using FluentResults;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace BoardProject.Domain.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ILogger<ProjectService> _logger;
        private readonly ConnectionFactory _eventBusConnectionFactory;
        private readonly EventBusSettings _settings;
        private readonly Publisher _publisher;

        public ProjectService(
            IProjectRepository projectRepository, 
            ILogger<ProjectService> logger, 
            ConnectionFactory eventBusConnectionFactory, 
            EventBusSettings settings)
        {
            _projectRepository = projectRepository;
            _logger = logger;
            _eventBusConnectionFactory = eventBusConnectionFactory;
            _settings = settings;
            _publisher = new Publisher(settings, eventBusConnectionFactory);
        }

        public async Task<IResult<ProjectResponse>> AddProjectAsync(AddProjectRequest request)
        {
            try
            {
                var validator = new AddProjectRequestValidator();
                var validationResult = await validator.ValidateAsync(request);

                if (!validationResult.IsValid)
                {
                    return (IResult<ProjectResponse>)Result.Fail(validationResult.Errors.Select(val => val.ErrorMessage).ToList());
                }

                var projectEntity = ProjectMapper.ToEntity(request);

                var project = _projectRepository.AddProject(projectEntity);

                if(project is null)
                {
                    return (IResult<ProjectResponse>)Result.Fail(string.Format("Bad request for project entity with Id {0}", projectEntity.ProjectId));
                }

                await _projectRepository.UnitOfWork.SaveChangesAsync();

                return (IResult<ProjectResponse>)Result.Ok(project.ToResponse);
            }
            catch (Exception ex)
            {
               _logger.LogError($"Exception was thrown from {nameof(AddProjectAsync)}");
                return (IResult<ProjectResponse>)Result.Fail(ex.Message);
            }
        }

        public async Task<IResult<ProjectResponse>> DeleteProjectAsync(DeleteProjectRequest request)
        {
            try
            {
                var validator = new DeleteProjectRequestValidator();
                var validationResult = await validator.ValidateAsync(request);

                if (!validationResult.IsValid)
                {
                    return (IResult<ProjectResponse>)Result.Fail(validationResult.Errors.Select(val => val.ErrorMessage).ToList());
                }

                var projectEntity = ProjectMapper.ToEntity(request);

                var project = _projectRepository.DeleteProject(projectEntity);

                if (project is null)
                {
                    return (IResult<ProjectResponse>)Result.Fail(string.Format("Bad request for project entity with Id {0}", projectEntity.ProjectId));
                }

                await _projectRepository.UnitOfWork.SaveChangesAsync();

                return (IResult<ProjectResponse>)Result.Ok(project.ToResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception was thrown from {nameof(DeleteProjectAsync)}");
                return (IResult<ProjectResponse>)Result.Fail(ex.Message);
            }
        }

        public async Task<IResult<ProjectResponse>> EditProjectAsync(EditProjectRequest request)
        {
            try
            {
                var validator = new EditProjectRequestValidator();
                var validationResult = await validator.ValidateAsync(request);

                if (!validationResult.IsValid)
                {
                    return (IResult<ProjectResponse>)Result.Fail(validationResult.Errors.Select(val => val.ErrorMessage).ToList());
                }

                var projectEntity = ProjectMapper.ToEntity(request);

                var project = _projectRepository.UpdateProject(projectEntity);

                if (project is null)
                {
                    return (IResult<ProjectResponse>)Result.Fail(string.Format("Bad request for project entity with Id {0}", projectEntity.ProjectId));
                }

                await _projectRepository.UnitOfWork.SaveChangesAsync();

                return (IResult<ProjectResponse>)Result.Ok(project.ToResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception was thrown from {nameof(EditProjectAsync)}");
                return (IResult<ProjectResponse>)Result.Fail(ex.Message);
            }
        }

        public async Task<IResult<ProjectResponse>> GetProjectAsync(GetProjectRequest request)
        {
            try
            {
                var validator = new GetProjectRequestValidator();
                var validationResult = await validator.ValidateAsync(request);

                if (!validationResult.IsValid)
                {
                    return (IResult<ProjectResponse>)Result.Fail(validationResult.Errors.Select(val => val.ErrorMessage).ToList());
                }

                var projectEntity = ProjectMapper.ToEntity(request);

                var project = await _projectRepository.GetAsyncByProjectId(projectEntity.ProjectId);

                if (project is null)
                {
                    return (IResult<ProjectResponse>)Result.Fail(string.Format("Bad request for project entity with Id {0}", projectEntity.ProjectId));
                }

                var isEventSend = IsGetAllJobsByProjectIdEventSend(new GetJobsByProjectIdEvent { Id = projectEntity.ProjectId });

                if(!isEventSend) 
                {
                    return (IResult<ProjectResponse>)Result.Fail($"Event not send from {nameof(GetProjectAsync)}");
                }

                return (IResult<ProjectResponse>)Result.Ok(project.ToResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception was thrown from {nameof(GetProjectAsync)}");
                return (IResult<ProjectResponse>)Result.Fail(ex.Message);
            }
        }

        public async Task<IResult<IEnumerable<ProjectResponse>>> GetProjectsAsync(int pageSize, int pageIndex)
        {
            try
            {
                var projects = await _projectRepository.GetAllProjectsAsync();

                var totalProjects = projects.Count();

                var projectsOnPage = projects.ToEnumerableResponse()
                    .OrderBy(c => c.Title)
                    .Skip(pageSize * pageIndex)
                    .Take(pageSize);

                var model = new PaginatedEntityResponseModel<ProjectResponse>(
                    pageIndex, pageSize, totalProjects, projectsOnPage);

                return (IResult<IEnumerable<ProjectResponse>>)Result.Ok(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception was thrown from {nameof(GetProjectsAsync)}");
                return (IResult<IEnumerable<ProjectResponse>>)Result.Fail(ex.Message);
            }
        }

        private bool IsGetAllJobsByProjectIdEventSend(GetJobsByProjectIdEvent message)
        {
            try
            {
                _publisher.PublishMessage(message);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogWarning("Unable to initialize the event bus: { message}", e.Message);
            }
            return false;
        }
    }
}
