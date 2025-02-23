using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HiveSpace.Domain.AggergateModels.SkuAggregate;
using HiveSpace.Domain.Enums;
using HiveSpace.Domain.Shared;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HiveSpace.Infrastructure.EntityConfigurations;
public class SkuEntityConfiguration : IEntityTypeConfiguration<Sku>
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public void Configure(EntityTypeBuilder<Sku> builder)
    {
        builder.ToTable("skus");

        builder.Property(o => o.Id)
            .UseHiLo("SkuSeq");

        builder.Ignore(o => o.DomainEvents);

        builder.Property(o => o.SkuNo)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(o => o.ProductId)
            .IsRequired();

        // Cấu hình SkuVariants để lưu JSON
        builder.Property(s => s.SkuVariants)
            .HasConversion(
                skuVariants => JsonSerializer.Serialize(skuVariants, _jsonSerializerOptions), // Serialize thành JSON
                json => JsonSerializer.Deserialize<List<SkuVariant>>(json ?? "[]", _jsonSerializerOptions) ?? new List<SkuVariant>() // Deserialize thành danh sách
            )
            .HasColumnType("jsonb") // PostgreSQL JSONB
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

        builder.Property(o => o.Quantity)
            .IsRequired();
    }
}
