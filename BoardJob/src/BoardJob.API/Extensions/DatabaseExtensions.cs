using BoardJob.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BoardJob.Api.Extensions
{
    public static class DatabaseExtensions
    {
        public static IServiceCollection AddDatabaseContext(this IServiceCollection services, string connectionString)
        {
            return services
                .AddEntityFrameworkSqlServer()
                .AddDbContext<ApplicationDbContext>(opt =>
                {
                    opt.UseSqlServer(
                        connectionString,
                        x =>
                        {
                            x.MigrationsAssembly(typeof(Program)
                                .GetTypeInfo()
                                .Assembly
                                .GetName().Name);
                        });
                });
        }
    }
}
