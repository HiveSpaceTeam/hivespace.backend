using Microsoft.EntityFrameworkCore;
using HiveSpace.Domain.AggergateModels.ShoppingCartAggregate;
using HiveSpace.Domain.Repositories;

namespace HiveSpace.Infrastructure.Repositories;

public class ShoppingCartRepository(HiveSpaceDbContext context) : BaseRepository<ShoppingCart, Guid>(context), IShoppingCartRepository
{
    public async Task<ShoppingCart?> GetShoppingCartByUserIdAsync(Guid userId)
    {
        return await _context.ShoppingCart.Where(x => x.CustomerId == userId)
            .Include(x => x.Items)
            .FirstOrDefaultAsync();
    }

    protected override IQueryable<ShoppingCart> ApplyIncludeDetail(IQueryable<ShoppingCart> query)
    {
        return query.Include(x => x.Items);
    }
}
