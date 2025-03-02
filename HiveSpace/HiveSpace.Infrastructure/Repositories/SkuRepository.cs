using HiveSpace.Domain.AggergateModels.SkuAggregate;
using HiveSpace.Domain.Repositories;

namespace HiveSpace.Infrastructure.Repositories;

public class SkuRepository(HiveSpaceDbContext context) : BaseRepository<Sku, int>(context), ISkuRepository
{


}
