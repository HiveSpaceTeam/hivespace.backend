using HiveSpace.Domain.AggergateModels.SkuAggregate;
using HiveSpace.Domain.Repositories;

namespace HiveSpace.Infrastructure.Repositories;

public class SkuRepository(NichoShopDbContext context) : BaseRepository<Sku, int>(context), ISkuRepository
{


}
