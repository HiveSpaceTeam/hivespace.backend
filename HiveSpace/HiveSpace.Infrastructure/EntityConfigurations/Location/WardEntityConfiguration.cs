using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HiveSpace.Domain.Location;

namespace HiveSpace.Infrastructure.EntityConfigurations.Location;

public class WardEntityConfiguration : IEntityTypeConfiguration<Ward>
{
    public void Configure(EntityTypeBuilder<Ward> builder)
    {
        builder.ToTable("wards");

        builder.HasKey(w => w.Code);

        builder.Property(w => w.Code)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(w => w.Name)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(w => w.NameEn)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(w => w.FullName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(w => w.FullNameEn)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(w => w.CodeName)
            .HasMaxLength(255)
            .IsRequired();

        builder.HasOne<District>()
            .WithMany()
            .HasForeignKey(w => w.DistrictCode);

        builder.HasOne<AdministrativeUnit>()
            .WithMany()
            .HasForeignKey(w => w.AdministrativeUnitId);
    }
}