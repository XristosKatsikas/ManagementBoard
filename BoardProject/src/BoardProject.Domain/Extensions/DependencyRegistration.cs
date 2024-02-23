using BoardProject.Domain.Services;
using BoardProject.Domain.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;


namespace BoardProject.Domain.Extensions
{
    public static class DependencyRegistration
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services
                .AddScoped<IProjectService, ProjectService>()
                .AddScoped<IUserService, UserService>();

            return services;
        }
    }
}
