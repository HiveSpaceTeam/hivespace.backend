using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HiveSpace.Domain.AggergateModels.UserAggregate;

namespace HiveSpace.Infrastructure.EntityConfigurations;
public class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.Ignore(o => o.DomainEvents);

        builder.Property(o => o.Id)
            .ValueGeneratedOnAdd();

        builder.Property(o => o.Id)
            .ValueGeneratedOnAdd();

        builder.Property(o => o.Email)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(o => o.FullName)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(o => o.UserName)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(o => o.PasswordHashed)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(o => o.Gender)
            .HasConversion<int>()
            .IsRequired(false);

        builder.Property(o => o.DateOfBirth)
            .HasConversion<DateTime>()
            .IsRequired(false);

        builder.OwnsOne(o => o.PhoneNumber, phoneBuilder =>
        {
            phoneBuilder.Property(p => p.Value)
                .HasColumnType("varchar(15)")
                .HasColumnName(nameof(User.PhoneNumber))
                .HasMaxLength(15)
                .IsRequired();
        });

        var navigation = builder.Metadata.FindNavigation(nameof(User.Addresses))!;
        navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
