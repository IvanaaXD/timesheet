using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeSheet.Domain.Entities;

namespace TimeSheet.Infrastructure.Data.Configurations
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(50);

            builder.Property(p => p.Description)
                    .IsRequired()
                    .HasMaxLength(2500);

            builder.HasOne(p => p.Client)
                .WithMany()
                .HasForeignKey(p => p.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.CurrentLead)
                   .WithMany() 
                   .HasForeignKey(p => p.CurrentLeadId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(p => !p.IsDeleted);
        }
    }
}