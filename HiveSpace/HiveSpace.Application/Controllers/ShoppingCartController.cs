using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HiveSpace.Application.Interfaces;
using HiveSpace.Application.Models.Dtos.Request.CartItem;
using HiveSpace.Application.Models.Dtos.Request.ShoppingCart;
using System.Net;

namespace HiveSpace.Application.Controllers;

[Route("api/v1/cart")]
[ApiController]
[Authorize]
public class ShoppingCartController : Controller
{
    private readonly IShoppingCartService _shoppingCartService;

    public ShoppingCartController(IShoppingCartService shoppingCartService)
    {
        _shoppingCartService = shoppingCartService;
    }

    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetShoppingCartByUserId()
    {
        var result = await _shoppingCartService.GetShoppingCartByUserIdAsync();
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ProducesResponseType((int)HttpStatusCode.UnprocessableContent)]
    public async Task<IActionResult> AddItemToCart([FromBody] AddItemToCartRequestDto param)
    {
        var result = await _shoppingCartService.AddItemToCartAsync(param);
        return Ok(result);
    }

    [HttpPut("items/quantity")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> UpdateCartItem(UpdateCartItemRequestDto updateCartItemRequestDto)
    {
        var result = await _shoppingCartService.UpdateCartItem(updateCartItemRequestDto);
        return Ok(result);
    }

    [HttpPut("items")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> UpdateCartItemMultiSelection(UpdateMultiCartItemSelectionDto updateSeletionDto)
    {
        var result = await _shoppingCartService.UpdateMultiSelection(updateSeletionDto);
        return Ok(result);
    }

    [HttpDelete("items/{cartItemId}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> DeleteCartItem(Guid cartItemId)
    {
        var result = await _shoppingCartService.DeleteCartItem(cartItemId);
        return Ok(result);
    }

    [HttpGet("checkout")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetCheckOut()
    {
        var result = await _shoppingCartService.GetCheckOutAsync();
        return Ok(result);
    }
}
