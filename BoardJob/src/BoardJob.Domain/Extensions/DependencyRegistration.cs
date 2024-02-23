using BoardJob.Domain.Services;
using BoardJob.Domain.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;


namespace BoardJob.Domain.Extensions
{
    public static class DependencyRegistration
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services
                .AddScoped<IUserService, UserService>();

            return services;
        }
    }
}
