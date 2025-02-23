using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HiveSpace.Domain.Location;

namespace HiveSpace.Infrastructure.EntityConfigurations.Location;

public class ProvinceEntityConfiguration : IEntityTypeConfiguration<Province>
{
    public void Configure(EntityTypeBuilder<Province> builder)
    {
        builder.ToTable("provinces");

        builder.HasKey(p => p.Code);

        builder.Property(p => p.Code)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(p => p.Name)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(p => p.NameEn)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(p => p.FullName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(p => p.FullNameEn)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(p => p.CodeName)
            .HasMaxLength(255)
            .IsRequired();
    }
}