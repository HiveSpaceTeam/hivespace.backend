using Microsoft.EntityFrameworkCore;
using HiveSpace.Domain.AggergateModels.ProductAggregate;
using HiveSpace.Domain.Repositories;

namespace HiveSpace.Infrastructure.Repositories
{
    public class ProductRepository(NichoShopDbContext context) : BaseRepository<Product, int>(context), IProductRepository
    {
        protected override IQueryable<Product> ApplyIncludeDetail(IQueryable<Product> query)
        {
            return query
                .Include(x => x.Categories)
                .Include(x => x.Skus)
                .Include(x => x.Variants);
        }
    }
}
