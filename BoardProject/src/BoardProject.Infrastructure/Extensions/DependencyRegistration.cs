using BoardProject.Domain.Entities;
using BoardProject.Domain.Repositories.Abstraction;
using BoardProject.Domain.Repositories.Abstractions;
using BoardProject.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace BoardProject.Infrastructure.Extensions
{
    public static class DependencyRegistration
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services
                .AddScoped<IProjectRepository, ProjectRepository>()
                .AddScoped<IGenericRepository<Project>, GenericRepository<Project>>()
                .AddScoped<IUserRepository, UserRepository>();

            return services;
        }
    }
}
