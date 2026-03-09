using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeSheet.Domain.Entities;

namespace TimeSheet.Infrastructure.Data.Configurations
{
    public class ProjectMemberConfiguration : IEntityTypeConfiguration<ProjectMember>
    {
        public void Configure(EntityTypeBuilder<ProjectMember> builder)
        {
            builder.HasKey(pm => new { pm.ProjectId, pm.MemberId });

            builder.HasOne(pm => pm.Project)
                   .WithMany(p => p.TeamMembers)
                   .HasForeignKey(pm => pm.ProjectId);

            builder.HasOne(pm => pm.Member)
                   .WithMany(m => m.ProjectMemberships)
                   .HasForeignKey(pm => pm.MemberId);
        }
    }
}