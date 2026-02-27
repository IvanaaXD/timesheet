using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeSheet.Domain.Entities;

namespace TimeSheet.Infrastructure.Data.Configurations
{
    public class ActivityConfiguration : IEntityTypeConfiguration<Activity>
    {
        public void Configure(EntityTypeBuilder<Activity> builder)
        {
            builder.Property(p => p.Description)
                    .IsRequired()
                    .HasMaxLength(2500);

            builder.Property(p => p.Date)
                    .IsRequired()
                    .HasColumnType("date")
                    .HasDefaultValueSql("CURRENT_DATE");

            builder.Property(p => p.Time)
                    .IsRequired();

            builder.Property(p => p.OverTime)
                    .IsRequired();

            builder.HasOne(a => a.Project)
                    .WithMany()
                    .HasForeignKey(a => a.ProjectId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Member)
                    .WithMany()
                    .HasForeignKey(a => a.MemberId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Category)
                    .WithMany()
                    .HasForeignKey(a => a.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}