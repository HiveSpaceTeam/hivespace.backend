using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HiveSpace.Domain.AggergateModels;
using HiveSpace.Domain.AggergateModels.ProductAggregate;

namespace HiveSpace.Infrastructure.EntityConfigurations;
public class ProductAttributeValueEntityConfiguration : IEntityTypeConfiguration<ProductAttributeValue>
{
    public void Configure(EntityTypeBuilder<ProductAttributeValue> builder)
    {
        builder.ToTable("product_attribute_values");

        builder.Property<int>("Id")
            .UseHiLo("ProductAttributeValueSeq");

        builder.Property<int>("ProductId")
            .IsRequired();

        builder.Property(o => o.AttributeId)
            .IsRequired();

        builder.Property(o => o.RawValue)
            .HasMaxLength(100);

        builder.Property(o => o.ValueId);

        builder.HasOne<AttributeProduct>()
            .WithMany()
            .HasForeignKey(o => o.AttributeId);
    }
}
