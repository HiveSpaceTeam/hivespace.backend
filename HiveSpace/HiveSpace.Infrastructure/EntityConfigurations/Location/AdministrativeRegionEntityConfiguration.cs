using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HiveSpace.Domain.Location;

namespace HiveSpace.Infrastructure.EntityConfigurations.Location;

public class AdministrativeRegionEntityConfiguration : IEntityTypeConfiguration<AdministrativeRegion>
{
    public void Configure(EntityTypeBuilder<AdministrativeRegion> builder)
    {
        builder.ToTable("administrative_regions");

        builder.HasKey(ar => ar.Id);

        builder.Property(ar => ar.Id)
            .IsRequired();

        builder.Property(ar => ar.Name)
            .HasMaxLength(255);

        builder.Property(ar => ar.NameEn)
            .HasMaxLength(255);

        builder.Property(ar => ar.CodeName)
            .HasMaxLength(255);

        builder.Property(ar => ar.CodeNameEn)
            .HasMaxLength(255);
    }
}