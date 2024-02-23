using BoardProject.Domain.Entities;
using BoardProject.Domain.Repositories.Abstraction;
using BoardProject.Infrastructure.SchemaDefinitions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BoardProject.Infrastructure
{
    /// <summary>
    /// Declare the identity data context by extending the IdentityDbContext class. 
    /// IdentityDbContext is used by EF Core to locate and access the data source used as the persistent user store.
    /// </summary>
    sealed public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<User>(options), IUnitOfWork
    {
        public const string DEFAULT_SCHEMA = "BoardProjectDatabase";

        public DbSet<Project> Projects { get; set; }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            await SaveChangesAsync(cancellationToken);
            return true;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new ProjectEntitySchemaDefinition());

            base.OnModelCreating(builder);
        }
    }
}
