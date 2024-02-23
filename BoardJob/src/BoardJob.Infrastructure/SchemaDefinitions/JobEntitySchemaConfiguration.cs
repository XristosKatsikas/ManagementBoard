using BoardJob.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace BoardJob.Infrastructure.SchemaDefinitions
{
    public class JobEntitySchemaConfiguration : IEntityTypeConfiguration<Job>
    { 
        public void Configure(EntityTypeBuilder<Job> builder)
        {
            builder.ToTable("Jobs", ApplicationDbContext.DEFAULT_SCHEMA);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
               .IsRequired();

            builder.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Progress)
                .IsRequired();

            builder.Property(x => x.StartDate)
                .IsRequired();

            builder.Property(x => x.FinishDate)
                .IsRequired();

            builder.Property(x => x.ProjectId)
                .IsRequired();
        }
    }
}
