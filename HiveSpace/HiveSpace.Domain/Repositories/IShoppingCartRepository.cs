using HiveSpace.Domain.AggergateModels.ShoppingCartAggregate;
using HiveSpace.Domain.Seedwork;

namespace HiveSpace.Domain.Repositories;

public interface IShoppingCartRepository : IRepository<ShoppingCart>
{
    Task<ShoppingCart?> GetShoppingCartByUserIdAsync(Guid userId);
}
