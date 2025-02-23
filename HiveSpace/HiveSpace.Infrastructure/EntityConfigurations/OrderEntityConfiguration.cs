using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HiveSpace.Domain.AggergateModels.OrderAggregate;
using HiveSpace.Domain.Enums;
using HiveSpace.Domain.Shared;

namespace HiveSpace.Infrastructure.EntityConfigurations;
public class OrderEntityConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");

        builder.Ignore(o => o.DomainEvents);

        builder.Property(o => o.Id)
            .ValueGeneratedOnAdd();

        builder.Property(o => o.CustomerId)
            .IsRequired();

        builder.Property(o => o.Status)
            .HasConversion<int>();


        builder.Property(o => o.OrderDate)
            .IsRequired();

        builder.Property(o => o.Discount)
            .HasColumnType("decimal(13,2)")
            .IsRequired();

        builder.Property(o => o.ShippingFee)
            .HasColumnType("decimal(13,2)")
            .IsRequired();

        builder.OwnsOne(o => o.ShippingAddress, shippingAddress =>
        {
            shippingAddress.Property(s => s.FullName)
                .HasColumnName($"{nameof(Order)}{nameof(ShippingAddress.FullName)}")
                .HasMaxLength(50)
                .IsRequired();

            shippingAddress.OwnsOne(s => s.PhoneNumber, phoneBuilder =>
            {
                phoneBuilder.Property(p => p.Value)
                    .HasColumnType("varchar(15)")
                    .HasColumnName($"{nameof(Order)}{nameof(ShippingAddress.PhoneNumber)}")
                    .HasMaxLength(15)
                    .IsRequired();
            });

            shippingAddress.Property(s => s.OtherDetails)
                .HasColumnName($"{nameof(Order)}{nameof(ShippingAddress.OtherDetails)}")
                .HasMaxLength(200)
                .IsRequired(false);

            shippingAddress.OwnsOne(s => s.Address, addressBuilder =>
            {
                addressBuilder.Property(a => a.Street)
                    .HasColumnName($"{nameof(Order)}{nameof(Address.Street)}")
                    .HasMaxLength(100)
                    .IsRequired();

                addressBuilder.Property(a => a.Ward)
                    .HasColumnName($"{nameof(Order)}{nameof(Address.Ward)}")
                    .HasMaxLength(50)
                    .IsRequired();

                addressBuilder.Property(a => a.District)
                    .HasColumnName($"{nameof(Order)}{nameof(Address.District)}")
                    .HasMaxLength(50)
                    .IsRequired();

                addressBuilder.Property(a => a.Province)
                    .HasColumnName($"{nameof(Order)}{nameof(Address.Province)}")
                    .HasMaxLength(50)
                    .IsRequired();

                addressBuilder.Property(a => a.Country)
                    .HasColumnName($"{nameof(Order)}{nameof(Address.Country)}")
                    .HasMaxLength(50)
                    .IsRequired();
            });
        });

        builder.Property(o => o.PaymentMethod)
              .HasConversion<int>();

    }
}
