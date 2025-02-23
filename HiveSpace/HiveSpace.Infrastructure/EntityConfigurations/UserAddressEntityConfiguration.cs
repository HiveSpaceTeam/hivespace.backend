using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HiveSpace.Domain.AggergateModels.UserAggregate;

namespace HiveSpace.Infrastructure.EntityConfigurations;
public class UserAddressEntityConfiguration : IEntityTypeConfiguration<UserAddress>
{
    public void Configure(EntityTypeBuilder<UserAddress> builder)
    {
        builder.ToTable("user_addresses");

        builder.Property(o => o.Id)
            .ValueGeneratedOnAdd();

        builder.Property(o => o.FullName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(o => o.Street)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(o => o.Ward)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(o => o.District)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(o => o.Province)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(o => o.Country)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(o => o.ZipCode)
            .HasMaxLength(10)
            .IsRequired(false);

        builder.OwnsOne(o => o.PhoneNumber, phoneBuilder =>
        {
            phoneBuilder.Property(p => p.Value)
                .HasColumnType("varchar(15)")
                .HasColumnName(nameof(UserAddress.PhoneNumber))
                .HasMaxLength(15)
                .IsRequired(); 
        });

        builder.Property(o => o.IsDefault)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property<Guid>("UserId");
    }
}
