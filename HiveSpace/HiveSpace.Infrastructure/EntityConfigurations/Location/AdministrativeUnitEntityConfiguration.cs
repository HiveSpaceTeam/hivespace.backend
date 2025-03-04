using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HiveSpace.Domain.Location;

namespace HiveSpace.Infrastructure.EntityConfigurations.Location;

public class AdministrativeUnitEntityConfiguration : IEntityTypeConfiguration<AdministrativeUnit>
{
    public void Configure(EntityTypeBuilder<AdministrativeUnit> builder)
    {
        builder.ToTable("administrative_units");

        builder.HasKey(au => au.Id);

        builder.Property(au => au.Id)
            .IsRequired();

        builder.Property(au => au.FullName)
            .HasMaxLength(255);

        builder.Property(au => au.FullNameEn)
            .HasMaxLength(255);

        builder.Property(au => au.ShortName)
            .HasMaxLength(255);

        builder.Property(au => au.ShortNameEn)
            .HasMaxLength(255);

        builder.Property(au => au.CodeName)
            .HasMaxLength(255);

        builder.Property(au => au.CodeNameEn)
            .HasMaxLength(255);
    }
}