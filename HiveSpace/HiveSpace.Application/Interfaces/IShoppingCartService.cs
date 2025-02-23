using HiveSpace.Application.Models.Dtos.Request.CartItem;
using HiveSpace.Application.Models.Dtos.Request.ShoppingCart;
using HiveSpace.Application.Models.ViewModels;

namespace HiveSpace.Application.Interfaces;

public interface IShoppingCartService
{
    Task<CartViewModel> GetShoppingCartByUserIdAsync();
    Task<bool> AddItemToCartAsync(AddItemToCartRequestDto param);
    Task<bool> UpdateCartItem(UpdateCartItemRequestDto updateCartItemRequestDto);
    Task<bool> DeleteCartItems(List<Guid> cartItemIds);
    Task<bool> UpdateMultiSelection(UpdateMultiCartItemSelectionDto updateSeletionDto);
    Task<CheckOutDto> GetCheckOutAsync();
    Task<bool> DeleteCartItem(Guid cartItemId);
}
