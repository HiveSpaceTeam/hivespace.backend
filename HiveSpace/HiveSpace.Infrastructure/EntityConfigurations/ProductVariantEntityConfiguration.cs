using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using HiveSpace.Domain.AggergateModels.ProductAggregate;

namespace HiveSpace.Infrastructure.EntityConfigurations;
public class ProductVariantEntityConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable("product_variants");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(pv => pv.Options)
            .HasConversion(
                // Convert List<ProductVariantOption> to JSON string using Newtonsoft.Json
                options => JsonConvert.SerializeObject(options, Formatting.None),
                // Convert JSON string back to List<ProductVariantOption> using Newtonsoft.Json
                json => JsonConvert.DeserializeObject<List<ProductVariantOption>>(json, new JsonConverter[] { new ProductVariantOptionConverter() }) ?? new List<ProductVariantOption>()
            )
            .HasColumnType("jsonb") // PostgreSQL JSONB column type
            .IsRequired();
    }
}

public class ProductVariantOptionConverter : JsonConverter<ProductVariantOption>
{
    public override ProductVariantOption? ReadJson(JsonReader reader, Type objectType, ProductVariantOption? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        // Read the value as a string
        var value = reader.Value?.ToString();
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        // Use the private constructor to create the object
        return new ProductVariantOption(value);
    }

    public override void WriteJson(JsonWriter writer, ProductVariantOption? value, JsonSerializer serializer)
    {
        // Write the value to JSON (assuming it's just a string)
        writer.WriteValue(value?.Value);
    }
}