using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeSheet.Domain.Entities;

namespace TimeSheet.Infrastructure.Data.Configurations
{
    public class MemberConfiguration : IEntityTypeConfiguration<Member>
    {
        public void Configure(EntityTypeBuilder<Member> builder)
        {
            builder.Property(m => m.Name)
                    .IsRequired()
                    .HasMaxLength(100);

            builder.Property(m => m.Username)
                    .IsRequired()
                    .HasMaxLength(100);

            builder.HasIndex(m => m.Username)
                    .IsUnique();

            builder.Property(m => m.Email)
                    .IsRequired()
                    .HasMaxLength(100);

            builder.HasIndex(m => m.Email)
                    .IsUnique();

            builder.Property(m => m.Password)
                    .IsRequired()
                    .HasMaxLength(100);

            builder.Property(m => m.HoursPerWeek)
                    .IsRequired();

            builder.Property(m => m.Status)
                    .IsRequired();

            builder.Property(m => m.Role)
                    .IsRequired();

            builder.HasMany<ProjectMember>()
                .WithOne(pl => pl.Member)
                .HasForeignKey(pl => pl.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(m => !m.IsDeleted);
        }
    }
}