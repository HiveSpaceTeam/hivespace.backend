using HiveSpace.Domain.AggergateModels;
using HiveSpace.Domain.Repositories;

namespace HiveSpace.Infrastructure.Repositories;

public class CategoryRepository(HiveSpaceDbContext context) : BaseRepository<Category, int>(context), ICategoryRepository
{
}
