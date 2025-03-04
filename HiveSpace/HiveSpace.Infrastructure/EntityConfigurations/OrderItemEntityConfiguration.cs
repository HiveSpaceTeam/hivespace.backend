using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HiveSpace.Domain.AggergateModels.OrderAggregate;
using HiveSpace.Domain.Enums;
using HiveSpace.Domain.Shared;

namespace HiveSpace.Infrastructure.EntityConfigurations;
public class OrderItemEntityConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("order_items");

        builder.Property(o => o.Id)
            .ValueGeneratedOnAdd();

        builder.Property(o => o.SkuId)
            .IsRequired();

        builder.Property(o => o.Quantity)
            .IsRequired();

        builder.OwnsOne(o => o.Price, priceBuilder =>
        {
            priceBuilder.Property(p => p.Amount)
                .HasColumnName(nameof(Money.Amount))
                .HasColumnType("decimal(13,2)")
                .IsRequired(); // Ensure the column is not nullable

            priceBuilder.Property(p => p.Currency)
                .HasColumnName(nameof(Money.Currency))
                .HasConversion(
                    v => v.ToString(),
                    dbCurrency => (Currency)Enum.Parse(typeof(Currency), dbCurrency)
                );
        });

        builder.Property(o => o.ProductName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(o => o.VariantName)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(o => o.Thumbnail)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property<Guid>("OrderId");
    }
}
