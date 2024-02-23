using BoardProject.Core;
using BoardProject.Core.Abstractions;
using BoardProject.Domain.Configurations;
using BoardProject.Domain.DTOs.Requests.Project;
using BoardProject.Domain.DTOs.Requests.Project.Validators;
using BoardProject.Domain.DTOs.Responses;
using BoardProject.Domain.Events.Project;
using BoardProject.Domain.Helper;
using BoardProject.Domain.Mappers;
using BoardProject.Domain.Repositories.Abstractions;
using BoardProject.Domain.Services.Abstractions;
using BoardProject.Domain.Services.RabbitMq;
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
                    return await FailedResponse.GetUnprocessableValidationResultAsync(validationResult);
                }

                var projectEntity = ProjectMapper.ToEntity(request);

                var project = _projectRepository.AddProject(projectEntity);

                if(project is null)
                {
                    return await FailedResponse.GetBadRequestResultAsync(projectEntity.ProjectId);
                }

                await _projectRepository.UnitOfWork.SaveChangesAsync();

                return Result<ProjectResponse>.CreateSuccessful(project.ToResponse());
            }
            catch (Exception ex)
            {
               _logger.LogError($"Exception was thrown from {nameof(AddProjectAsync)}");
                return Result<ProjectResponse>.CreateFailed(
                    _logger,
                    ResultCode.BadGateway,
                    ex.Message);
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
                    return await FailedResponse.GetUnprocessableValidationResultAsync(validationResult);
                }

                var projectEntity = ProjectMapper.ToEntity(request);

                var project = _projectRepository.DeleteProject(projectEntity);

                if (project is null)
                {
                    return await FailedResponse.GetBadRequestResultAsync(projectEntity.ProjectId);
                }

                await _projectRepository.UnitOfWork.SaveChangesAsync();

                return Result<ProjectResponse>.CreateSuccessful(project.ToResponse());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception was thrown from {nameof(DeleteProjectAsync)}");
                return Result<ProjectResponse>.CreateFailed(
                    _logger,
                    ResultCode.BadGateway,
                    ex.Message);
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
                    return await FailedResponse.GetUnprocessableValidationResultAsync(validationResult);
                }

                var projectEntity = ProjectMapper.ToEntity(request);

                var project = _projectRepository.UpdateProject(projectEntity);

                if (project is null)
                {
                    return await FailedResponse.GetBadRequestResultAsync(projectEntity.ProjectId);
                }

                await _projectRepository.UnitOfWork.SaveChangesAsync();

                return Result<ProjectResponse>.CreateSuccessful(project.ToResponse());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception was thrown from {nameof(EditProjectAsync)}");
                return Result<ProjectResponse>.CreateFailed(
                    _logger,
                    ResultCode.BadGateway,
                    ex.Message);
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
                    return await FailedResponse.GetUnprocessableValidationResultAsync(validationResult);
                }

                var projectEntity = ProjectMapper.ToEntity(request);

                var project = await _projectRepository.GetAsyncByProjectId(projectEntity.ProjectId);

                if (project is null)
                {
                    return await FailedResponse.GetBadRequestResultAsync(projectEntity.ProjectId);
                }

                var isEventSend = IsGetAllJobsByProjectIdEventSend(new GetJobsByProjectIdEvent { Id = request.Id });

                if(!isEventSend) 
                {
                    return await FailedResponse.GetUnprocessableEventResultAsync();
                }

                return Result<ProjectResponse>.CreateSuccessful(project.ToResponse());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception was thrown from {nameof(GetProjectAsync)}");
                return Result<ProjectResponse>.CreateFailed(
                    _logger,
                    ResultCode.BadGateway,
                    ex.Message);
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

                return Result<IEnumerable<ProjectResponse>>.CreateSuccessful(model.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception was thrown from {nameof(GetProjectsAsync)}");
                return Result<IEnumerable<ProjectResponse>>.CreateFailed(
                    _logger,
                    ResultCode.BadGateway,
                    ex.Message);
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
