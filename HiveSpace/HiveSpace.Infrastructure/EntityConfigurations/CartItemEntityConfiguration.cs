using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HiveSpace.Domain.AggergateModels.ShoppingCartAggregate;

namespace HiveSpace.Infrastructure.EntityConfigurations;
public class CartItemEntityConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("cart_items");

        builder.Property(o => o.Id)
            .ValueGeneratedOnAdd();

        builder.Property(o => o.SkuId)
            .IsRequired();

        builder.Property(o => o.Quantity)
            .IsRequired();

        builder.Property(o => o.IsSelected)
            .IsRequired();

        builder.Property<Guid>("ShoppingCartId");
    }
}
