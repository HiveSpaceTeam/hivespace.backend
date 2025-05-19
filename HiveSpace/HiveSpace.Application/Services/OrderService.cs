using HiveSpace.Application.Helpers;
using HiveSpace.Application.Interfaces;
using HiveSpace.Application.Models.Dtos.Request.Paging;
using HiveSpace.Application.Models.Dtos.Request.ShoppingCart;
using HiveSpace.Common.Interface;
using HiveSpace.Domain.AggergateModels.OrderAggregate;
using HiveSpace.Domain.AggergateModels.UserAggregate;
using HiveSpace.Domain.Enums;
using HiveSpace.Domain.Exceptions;
using HiveSpace.Domain.Repositories;
using HiveSpace.Domain.Shared;

namespace HiveSpace.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserAddressService _userAddressService;
        private readonly IUserContext _userContext;
        private readonly ISkuService _skuService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ITransactionService _transactionService;

        public OrderService(IUserContext userContext, IOrderRepository orderRepository, ISkuService skuSerive, IShoppingCartService shoppingCartService, IUserAddressService userAddressService, ITransactionService transactionService)
        {
            _userContext = userContext;
            _orderRepository = orderRepository;
            _skuService = skuSerive;
            _shoppingCartService = shoppingCartService;
            _userAddressService = userAddressService;
            _transactionService = transactionService;
        }
        public async Task<Guid> CreateOrder(CreateOrderRequestDto param)
        {
            Guid orderId = Guid.Empty;

            await _transactionService.InTransactionScopeAsync(async (transaction) =>
            {
                UserAddress userAddress = await _userAddressService.GetByIdAsync(param.UserAddressID);
                ShippingAddressProps shippingAddressProps = new ShippingAddressProps
                {
                    FullName = userAddress.FullName,
                    PhoneNumber = userAddress.PhoneNumber.Value,
                    OtherDetails = "",
                    Street = userAddress.Street,
                    Ward = userAddress.Ward,
                    District = userAddress.District,
                    Province = userAddress.Province,
                    Country = userAddress.Country
                };

                var cart = await _shoppingCartService.GetShoppingCartByUserIdAsync();
                var products = cart.Items.FindAll(x => x.IsSelected);

                var filtersWithComparison = new Dictionary<string, FilterItem>
                {
                    { "Id",

                        new FilterItem
                        {
                            Value=products.Select(x=>x.SkuId),
                            Comparison=SqlOperator.In
                        }
                    },
                };

                var skus = await _skuService.GetByFitlers(filtersWithComparison) ?? throw ExceptionHelper.NotFoundException(ApplicationErrorCode.NotFoundSku);

                bool isOutOfStock = skus.Any(x =>
                {
                    return products.Any(product => product.SkuId == x.Id && product.Quantity > x.Quantity);
                });

                if (isOutOfStock)
                {
                    throw ExceptionHelper.DomainException(ApplicationErrorCode.OutOfStock);
                }
                List<OrderItemProps> orderItemProps = products.Select(product =>
                {
                    return new OrderItemProps
                    {
                        SkuId = product.SkuId,
                        ProductName = product.ProductName,
                        VariantName = product.ProductVariantName,
                        Thumbnail = "https://down-vn.img.susercontent.com/file/vn-11134207-7ras8-m4k9lc59h3v355@resize_w80_nl.webp",
                        Quantity = product.Quantity,
                        Amount = (double)product.Amount,
                        Currency = Currency.VND,
                    };
                }).ToList();

                //Transaction
                var order = new Order(
                    _userContext.UserId,
                    0,
                    0,
                    DateTimeOffset.UtcNow,
                    shippingAddressProps,
                    param.PaymentMethod,
                    orderItemProps
                    );
                _orderRepository.Add(order);

                //Giảm số lượng trong kho tương ứng với số lượng trong đơn hàng
                await _skuService.UpdateSkus(skus.Select(sku =>
                {
                    var product = products.Find(product => product.SkuId == sku.Id) ?? throw ExceptionHelper.NotFoundException(ApplicationErrorCode.NotFoundSku);
                    sku.UpdateQuantity(sku.Quantity - product.Quantity);
                    return sku;
                }).ToList());

                // Xóa trong cart
                List<Guid> cartItemIds = products.Select(x => x.Id).ToList();
                await _shoppingCartService.DeleteCartItems(cartItemIds);
                orderId = order.Id;
            });
            
            return orderId;
        }

        public async Task<List<Order>> GetPaging(PagingRequestDto param)
        {

            return await _orderRepository.GetPaging(param.PageNumber, param.PageSize, param.Filters, true);
        }
    }
}
