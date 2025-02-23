using HiveSpace.Domain.AggergateModels;
using HiveSpace.Domain.Repositories;

namespace HiveSpace.Infrastructure.Repositories;

public class CategoryRepository(NichoShopDbContext context) : BaseRepository<Category, int>(context), ICategoryRepository
{
}
