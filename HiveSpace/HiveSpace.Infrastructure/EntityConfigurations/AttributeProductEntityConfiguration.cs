using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HiveSpace.Domain.AggergateModels;

namespace HiveSpace.Infrastructure.EntityConfigurations;
public class AttributeProductEntityConfiguration : IEntityTypeConfiguration<AttributeProduct>
{
    public void Configure(EntityTypeBuilder<AttributeProduct> builder)
    {
        builder.ToTable("attributes");

        builder.Property<int>("Id")
            .UseHiLo("AttributeProductSeq");

        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.ValueId)
            .IsRequired();

        builder.Property(a => a.DisplayName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.ParentId)
            .IsRequired(false);

        builder.Property(a => a.Mandatory)
            .IsRequired();

        builder.HasOne(a => a.Parent)
            .WithMany(a => a.Children)
            .HasForeignKey(a => a.ParentId)
            .IsRequired(false);

        builder.HasMany(p => p.Categories)
               .WithMany(c => c.Attributes)
               .UsingEntity(j => j.ToTable("attribute_categories"));
    }
}