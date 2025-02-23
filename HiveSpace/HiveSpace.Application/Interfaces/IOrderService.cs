using HiveSpace.Application.Models.Dtos.Request.Paging;
using HiveSpace.Application.Models.Dtos.Request.ShoppingCart;
using HiveSpace.Domain.AggergateModels.OrderAggregate;

namespace HiveSpace.Application.Interfaces
{
    public interface IOrderService
    {
        Task<Guid> CreateOrder(CreateOrderRequestDto param);
        Task<List<Order>> GetPaging(PagingRequestDto param);
    }
}
