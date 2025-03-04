using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HiveSpace.Domain.Location;

namespace HiveSpace.Infrastructure.EntityConfigurations.Location;

public class DistrictEntityConfiguration : IEntityTypeConfiguration<District>
{
    public void Configure(EntityTypeBuilder<District> builder)
    {
        builder.ToTable("districts");

        builder.HasKey(d => d.Code);

        builder.Property(d => d.Code)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(d => d.Name)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(d => d.NameEn)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(d => d.FullName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(d => d.FullNameEn)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(d => d.CodeName)
            .HasMaxLength(255)
            .IsRequired();

        builder.HasOne<Province>()
            .WithMany()
            .HasForeignKey(d => d.ProvinceCode);

        builder.HasOne<AdministrativeUnit>()
            .WithMany()
            .HasForeignKey(d => d.AdministrativeUnitId);
    }
}