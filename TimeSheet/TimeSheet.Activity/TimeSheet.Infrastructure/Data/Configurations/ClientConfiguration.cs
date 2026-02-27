using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeSheet.Domain.Entities;

namespace TimeSheet.Infrastructure.Data.Configurations
{
    public class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(50);

            builder.HasIndex(c => c.Name)
                    .IsUnique();

            builder.Property(c => c.Address)
                    .IsRequired()
                    .HasMaxLength(500);

            builder.Property(c => c.City)
                    .IsRequired()
                    .HasMaxLength(255);

            builder.Property(c => c.Zip)
                    .IsRequired()
                    .HasMaxLength(20);

            builder.HasOne(c => c.Country)
                    .WithMany()
                    .HasForeignKey(c => c.CountryId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(c => !c.IsDeleted);
        }
    }
}