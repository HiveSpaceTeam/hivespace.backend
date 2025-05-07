using FluentValidation;
using HiveSpace.Application.Interfaces;
using HiveSpace.Application.Models.Dtos.Request.CartItem;
using HiveSpace.Application.Models.Dtos.Request.ShoppingCart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HiveSpace.Application.Controllers;

[Route("api/v1/cart")]
[ApiController]
[Authorize]
public class ShoppingCartController : Controller
{
    private readonly IShoppingCartService _shoppingCartService;
    private readonly IValidator<AddItemToCartRequestDto> _addItemToCartValidator;
    private readonly IValidator<UpdateCartItemRequestDto> _updateCartItemValidator;
    private readonly IValidator<UpdateMultiCartItemSelectionDto> _updateMultiCartItemSelectionValidator;

    public ShoppingCartController(
        IShoppingCartService shoppingCartService,
        IValidator<AddItemToCartRequestDto> addItemToCartValidator,
        IValidator<UpdateCartItemRequestDto> updateCartItemValidator,
        IValidator<UpdateMultiCartItemSelectionDto> updateMultiCartItemSelectionValidator)
    {
        _shoppingCartService = shoppingCartService;
        _addItemToCartValidator = addItemToCartValidator;
        _updateCartItemValidator = updateCartItemValidator;
        _updateMultiCartItemSelectionValidator = updateMultiCartItemSelectionValidator;
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
        var validationResult = _addItemToCartValidator.Validate(param);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var result = await _shoppingCartService.AddItemToCartAsync(param);
        return Ok(result);
    }

    [HttpPut("items/quantity")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> UpdateCartItem([FromBody] UpdateCartItemRequestDto updateCartItemRequestDto)
    {
        var validationResult = _updateCartItemValidator.Validate(updateCartItemRequestDto);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var result = await _shoppingCartService.UpdateCartItem(updateCartItemRequestDto);
        return Ok(result);
    }

    [HttpPut("items/multi-selection")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> UpdateCartItemMultiSelection([FromBody] UpdateMultiCartItemSelectionDto updateSeletionDto)
    {
        var validationResult = _updateMultiCartItemSelectionValidator.Validate(updateSeletionDto);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

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
