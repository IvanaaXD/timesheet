using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeSheet.Domain.Entities;

namespace TimeSheet.Infrastructure.Data.Configurations
{
    public class ProjectLeadConfiguration : IEntityTypeConfiguration<ProjectLead>
    {
        public void Configure(EntityTypeBuilder<ProjectLead> builder)
        {
            builder.HasKey(pl => new { pl.ProjectId, pl.MemberId });

            builder.HasOne(pl => pl.Project)
                   .WithMany() 
                   .HasForeignKey(pl => pl.ProjectId);

            builder.HasOne(pl => pl.Member)
                   .WithMany(m => m.LeadingAssignments) 
                   .HasForeignKey(pl => pl.MemberId);
        }
    }
}