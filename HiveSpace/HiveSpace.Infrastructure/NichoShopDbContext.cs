using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using HiveSpace.Domain.AggergateModels;
using HiveSpace.Domain.AggergateModels.OrderAggregate;
using HiveSpace.Domain.AggergateModels.ProductAggregate;
using HiveSpace.Domain.AggergateModels.ShoppingCartAggregate;
using HiveSpace.Domain.AggergateModels.SkuAggregate;
using HiveSpace.Domain.AggergateModels.UserAggregate;
using HiveSpace.Domain.Location;
using HiveSpace.Infrastructure.EntityConfigurations;
using HiveSpace.Infrastructure.EntityConfigurations.Location;

namespace HiveSpace.Infrastructure;
public class NichoShopDbContext(DbContextOptions<NichoShopDbContext> options, IConfiguration configuration) : DbContext(options)
{

    private IConfiguration _configuration = configuration;

    public DbSet<Category> Category { get; set; }
    public DbSet<User> User { get; set; }
    public DbSet<UserAddress> UserAddress { get; set; }
    public DbSet<AttributeProduct> AttributeProduct { get; set; }
    public DbSet<ProductAttributeValue> ProductAttributeValue { get; set; }
    public DbSet<Product> Product { get; set; }
    public DbSet<ProductVariant> ProductVariant { get; set; }
    public DbSet<ProductCategory> ProductCategory { get; set; }
    public DbSet<ProductImage> ProductImage { get; set; }
    public DbSet<CartItem> CartItem { get; set; }
    public DbSet<ShoppingCart> ShoppingCart { get; set; }
    public DbSet<Order> Order { get; set; }
    public DbSet<OrderItem> OrderItem { get; set; }
    public DbSet<Sku> Sku { get; set; }


    #region location
    public DbSet<Province> Province { get; set; }
    public DbSet<District> District { get; set; }
    public DbSet<Ward> Ward { get; set; }
    public DbSet<AdministrativeUnit> AdministrativeUnit { get; set; }
    public DbSet<AdministrativeRegion> AdministrativeRegions { get; set; }
    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasSequence<int>("ProductSeq")
            .StartsAt(10_000_000)
            .IncrementsBy(10);

        modelBuilder.HasSequence<int>("ProductAttributeValueSeq")
            .StartsAt(100_000)
            .IncrementsBy(100);

        modelBuilder.HasSequence<int>("SkuSeq")
            .StartsAt(100_000)
            .IncrementsBy(10);

        modelBuilder.HasSequence<int>("AttributeProductSeq")
            .StartsAt(100_000)
            .IncrementsBy(100);

        modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserAddressEntityConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProductEntityConfiguration());
        modelBuilder.ApplyConfiguration(new AttributeProductEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProductVariantEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProductCategoryEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProductImageEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProductAttributeValueEntityConfiguration());
        modelBuilder.ApplyConfiguration(new CartItemEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ShoppingCartEntityConfiguration());
        modelBuilder.ApplyConfiguration(new OrderEntityConfiguration());
        modelBuilder.ApplyConfiguration(new OrderItemEntityConfiguration());
        modelBuilder.ApplyConfiguration(new SkuEntityConfiguration());

        #region location
        modelBuilder.ApplyConfiguration(new ProvinceEntityConfiguration());
        modelBuilder.ApplyConfiguration(new DistrictEntityConfiguration());
        modelBuilder.ApplyConfiguration(new WardEntityConfiguration());
        modelBuilder.ApplyConfiguration(new AdministrativeUnitEntityConfiguration());
        modelBuilder.ApplyConfiguration(new AdministrativeRegionEntityConfiguration());
        #endregion
    }
}
