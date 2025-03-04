using Microsoft.EntityFrameworkCore;
using HiveSpace.Domain.AggergateModels.OrderAggregate;
using HiveSpace.Domain.AggergateModels.ProductAggregate;
using HiveSpace.Domain.Repositories;

namespace HiveSpace.Infrastructure.Repositories
{
    public class OrderRepository(HiveSpaceDbContext context) : BaseRepository<Order, Guid>(context), IOrderRepository
    {
        protected override IQueryable<Order> ApplyIncludeDetail(IQueryable<Order> query)
        {
            return query
                .Include(x => x.Items);
        }
    }
}
