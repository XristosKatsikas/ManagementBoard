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
                .AddScoped<IUserRepository, UserRepository>();

            return services;
        }
    }
}