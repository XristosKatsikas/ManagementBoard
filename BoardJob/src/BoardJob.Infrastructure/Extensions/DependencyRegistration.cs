﻿using BoardJob.Domain.Repositories.Abstractions;
using BoardJob.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace BoardJob.Infrastructure.Extensions
{
    public static class DependencyRegistration
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services
                .AddScoped<IJobRepository, JobRepository>()
                .AddScoped<IUserRepository, UserRepository>();

            return services;
        }
    }
}
