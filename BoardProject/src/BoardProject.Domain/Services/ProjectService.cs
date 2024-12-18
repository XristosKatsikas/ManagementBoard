﻿using BoardProject.Core;
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
        private readonly RmqPublisher _publisher;

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
            _publisher = new RmqPublisher(settings, eventBusConnectionFactory);
        }

        public async Task<IResult<ProjectResponse>> AddProjectAsync(AddProjectRequest request)
        {
            var validator = new AddProjectRequestValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.Errors.Select(val => val.ErrorMessage).ToList();
                _logger.LogError($"Validation errors occurred in ProjectService.{nameof(AddProjectAsync)}: " +
                    $"{string.Join(", ", errorMessages)}");

                return Result.Fail<ProjectResponse>(FailedResultMessage.RequestValidation);
            }

            try
            {
                var projectEntity = ProjectMapper.ToEntity(request);

                var project = _projectRepository.AddProject(projectEntity);

                if(project is null)
                {
                    _logger.LogError($"Post data from ProjectService.{nameof(AddProjectAsync)} has failed.");
                    return Result.Fail<ProjectResponse>(FailedResultMessage.Unprocessable);
                }

                await _projectRepository.UnitOfWork.SaveChangesAsync();

                return Result.Ok(project.ToResponse());
            }
            catch (Exception ex)
            {
                _logger.LogError($"ProjectService.{nameof(AddProjectAsync)} has failed with exception message: {ex.Message}");
                return Result.Fail<ProjectResponse>(FailedResultMessage.Exception);
            }
        }

        public async Task<IResult<bool>> DeleteProjectAsync(DeleteProjectRequest request)
        {
            var validator = new DeleteProjectRequestValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.Errors.Select(val => val.ErrorMessage).ToList();
                _logger.LogError($"Validation errors occurred in ProjectService.{nameof(DeleteProjectAsync)}: " +
                    $"{string.Join(", ", errorMessages)}");

                return Result.Fail<bool>(FailedResultMessage.RequestValidation);
            }

            try
            {
                var projectEntity = ProjectMapper.ToEntity(request);

                var isProjectDeleted = _projectRepository.DeleteProject(projectEntity);

                if (!isProjectDeleted)
                {
                    _logger.LogError($"Delete data from ProjectService.{nameof(DeleteProjectAsync)} has failed.");
                    return Result.Fail<bool>(FailedResultMessage.NotFound);
                }

                await _projectRepository.UnitOfWork.SaveChangesAsync();

                return Result.Ok(isProjectDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError($"ProjectService.{nameof(DeleteProjectAsync)} has failed with exception message: {ex.Message}");
                return Result.Fail<bool>(FailedResultMessage.Exception);
            }
        }

        public async Task<IResult<ProjectResponse>> EditProjectAsync(EditProjectRequest request)
        {

            var validator = new EditProjectRequestValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.Errors.Select(val => val.ErrorMessage).ToList();
                _logger.LogError($"Validation errors occurred in ProjectService.{nameof(EditProjectAsync)}: " +
                    $"{string.Join(", ", errorMessages)}");

                return Result.Fail<ProjectResponse>(FailedResultMessage.RequestValidation);
            }

            try
            {
                var projectEntity = ProjectMapper.ToEntity(request);

                var project = _projectRepository.UpdateProject(projectEntity);

                if (project is null)
                {
                    _logger.LogError($"Update data from ProjectService.{nameof(EditProjectAsync)} has failed.");
                    return Result.Fail<ProjectResponse>(FailedResultMessage.Unprocessable);
                }

                await _projectRepository.UnitOfWork.SaveChangesAsync();

                return Result.Ok(project.ToResponse());
            }
            catch (Exception ex)
            {
                _logger.LogError($"ProjectService.{nameof(EditProjectAsync)} has failed with exception message: {ex.Message}");
                return Result.Fail<ProjectResponse>(FailedResultMessage.Exception);
            }
        }

        public async Task<IResult<ProjectResponse>> GetProjectAsync(GetProjectRequest request)
        {
            var validator = new GetProjectRequestValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.Errors.Select(val => val.ErrorMessage).ToList();
                _logger.LogError($"Validation errors occurred in ProjectService.{nameof(GetProjectAsync)}: " +
                    $"{string.Join(", ", errorMessages)}");

                return Result.Fail<ProjectResponse>(FailedResultMessage.RequestValidation);
            }

            try
            {
                var projectEntity = ProjectMapper.ToEntity(request);

                var project = await _projectRepository.GetAsyncByProjectId(projectEntity.ProjectId);

                if (project is null)
                {
                    _logger.LogInformation($"No data to fetch in ProjectService.{nameof(GetProjectAsync)}");
                    return Result.Fail<ProjectResponse>(FailedResultMessage.NotFound);
                }

                var isEventSend = await IsGetAllJobsByProjectIdEventSendAsync(new GetJobsByProjectIdEvent { Id = projectEntity.ProjectId });

                if(!isEventSend) 
                {
                    return (IResult<ProjectResponse>)Result.Fail($"Event not send from {nameof(GetProjectAsync)}");
                }

                return (IResult<ProjectResponse>)Result.Ok(project.ToResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"ProjectService.{nameof(GetProjectAsync)} has failed: {ex.Message}");
                return Result.Fail<ProjectResponse>(FailedResultMessage.Exception);
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

                return Result.Ok(model.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"ProjectService.{nameof(GetProjectsAsync)} has failed: {ex.Message}");
                return Result.Fail<IEnumerable<ProjectResponse>>(FailedResultMessage.Exception);
            }
        }

        private async Task<bool> IsGetAllJobsByProjectIdEventSendAsync(GetJobsByProjectIdEvent evt)
        {
            try
            {
                await _publisher.PublishAsync(evt);
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