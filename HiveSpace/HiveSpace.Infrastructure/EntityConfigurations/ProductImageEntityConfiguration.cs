using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HiveSpace.Domain.AggergateModels.ProductAggregate;

namespace HiveSpace.Infrastructure.EntityConfigurations;
public class ProductImageEntityConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.ToTable("product_images");

        builder.HasKey(o => new { o.ProductId, o.FileId });
    }
}
