using HiveSpace.Application.Helpers;
using HiveSpace.Application.Interfaces;
using HiveSpace.Application.Models.Dtos.Request.CartItem;
using HiveSpace.Application.Models.Dtos.Request.ShoppingCart;
using HiveSpace.Application.Models.ViewModels;
using HiveSpace.Application.Queries;
using HiveSpace.Common.Exceptions;
using HiveSpace.Common.Interface;
using HiveSpace.Domain.AggergateModels.ShoppingCartAggregate;
using HiveSpace.Domain.AggergateModels.SkuAggregate;
using HiveSpace.Domain.Enums;
using HiveSpace.Domain.Exceptions;
using HiveSpace.Domain.Repositories;

namespace HiveSpace.Application.Services;

public class ShoppingCartService : IShoppingCartService
{
    private readonly IQueryService _queryService;
    private readonly IUserContext _userContext;
    private readonly IUserAddressService _userAddressService;
    private readonly IShoppingCartRepository _shoppingCartRepository;
    private readonly ISkuRepository _skuRepository;

    public ShoppingCartService(IUserContext userContext, IQueryService queryService, IShoppingCartRepository shoppingCartRepository, ISkuRepository skuRepository, IUserAddressService userAddressService)
    {
        _queryService = queryService;
        _userContext = userContext;
        _userAddressService = userAddressService;
        _shoppingCartRepository = shoppingCartRepository;
        _skuRepository = skuRepository;
    }

    public async Task<CartViewModel> GetShoppingCartByUserIdAsync()
    {
        var userId = _userContext.UserId;
        var result = new CartViewModel();

        var shoppingCart = await _shoppingCartRepository.GetShoppingCartByUserIdAsync(userId);

        if (shoppingCart is not null)
        {
            var cartItems = await _queryService.GetCartItemViewModelsAsync(userId);
            result.Id = shoppingCart.Id;
            result.Items = cartItems;
        }
        return result;
    }

    public async Task<bool> UpdateCartItem(UpdateCartItemRequestDto updateCartItemRequestDto)
    {
        var cart = await _shoppingCartRepository.GetByIdAsync(updateCartItemRequestDto.CartId, includeDetail: true)
            ?? throw ExceptionHelper.NotFoundException(ApplicationErrorCode.UserNotFound);
        if (!await IsValidQuantitySkuAsync(updateCartItemRequestDto.Quantity, updateCartItemRequestDto.Id))
        {
            throw ExceptionHelper.DomainException(ApplicationErrorCode.InvalidQuantity, nameof(Sku.Quantity), updateCartItemRequestDto.Quantity);
        }

        cart.UpdateCartItem(new CartItem(updateCartItemRequestDto.Id, updateCartItemRequestDto.Quantity));
        return await _shoppingCartRepository.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteCartItems(List<Guid> cartItemIds)
    {
        var userId = _userContext.UserId;
        var shoppingCart = await _shoppingCartRepository.GetShoppingCartByUserIdAsync(userId) 
            ?? throw ExceptionHelper.NotFoundException(ApplicationErrorCode.UserNotFound);
        shoppingCart.RemoveItems(cartItemIds);
        return await _shoppingCartRepository.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateMultiSelection(UpdateMultiCartItemSelectionDto updateSeletionDto)
    {
        var cart = await _shoppingCartRepository.GetByIdAsync(updateSeletionDto.CartId, includeDetail: true)
            ?? throw ExceptionHelper.NotFoundException(ApplicationErrorCode.UserNotFound);

        cart.UpdateSelectionCartItem(updateSeletionDto.SkuIds, updateSeletionDto.IsSelected);
        return await _shoppingCartRepository.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteCartItem(Guid cartItemId)
    {
        var userId = _userContext.UserId;
        var shoppingCart = await _shoppingCartRepository.GetShoppingCartByUserIdAsync(userId)
            ?? throw ExceptionHelper.NotFoundException(ApplicationErrorCode.UserNotFound);

        shoppingCart.RemoveItem(cartItemId);
        return await _shoppingCartRepository.SaveChangesAsync() > 0;

    }

    public async Task<bool> AddItemToCartAsync(AddItemToCartRequestDto param)
    {
        var userId = _userContext.UserId;
        var cart = await _shoppingCartRepository.GetShoppingCartByUserIdAsync(userId);

        if (cart is null)
        {
            cart = new ShoppingCart(userId);
            _shoppingCartRepository.Add(cart);
        }

        var itemCartAdd = cart.AddItem(param.Quantity, param.SkuId, param.IsSelected);

        if (!await IsValidQuantitySkuAsync(itemCartAdd.Quantity, itemCartAdd.SkuId))
        {
            throw ExceptionHelper.DomainException(ApplicationErrorCode.InvalidQuantity, nameof(Sku.Quantity), param.Quantity);
        }

        return await _shoppingCartRepository.SaveChangesAsync() > 0;
    }

    public async Task<CheckOutDto> GetCheckOutAsync()
    {
        var result = new CheckOutDto();

        var addresses = await _userAddressService.GetUserAddressAsync();
        var addressDefault = addresses.Find(x => x.IsDefault);
        result.Address = addressDefault;

        var products = await GetShoppingCartByUserIdAsync();
        result.Products = products.Items.FindAll(x => x.IsSelected);

        return result;
    }

    /// <summary>
    /// Check if the quantity of a SKU is valid
    /// </summary>
    /// <param name="quantity"></param>
    /// <param name="skuId"></param>
    /// <returns></returns>
    private async Task<bool> IsValidQuantitySkuAsync(int quantity, int skuId)
    {
        var sku = await _skuRepository.GetByIdAsync(skuId);
        if (sku is not null)
        {
            return sku.Quantity >= quantity;
        }
        return false;
    }
}
