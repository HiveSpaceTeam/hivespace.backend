using HiveSpace.Application.Models.Dtos.Request.UserAddress;
using HiveSpace.Application.Models.ViewModels;

namespace HiveSpace.Application.Models.Dtos.Request.ShoppingCart;

public class CheckOutDto
{
    public UserAddressDto? Address { get; set; }
    public List<CartItemViewModel> Products { get; set; }
}
