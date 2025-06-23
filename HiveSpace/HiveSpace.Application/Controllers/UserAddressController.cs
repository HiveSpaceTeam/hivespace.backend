using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HiveSpace.Application.Helpers;
using HiveSpace.Application.Interfaces;
using HiveSpace.Application.Models.Dtos.Request.UserAddress;
using System.Net;

namespace HiveSpace.Application.Controllers;

[Route("api/v1/users/address")]
[ApiController]
[Authorize]
public class UserAddressController : ControllerBase
{
    private readonly IUserAddressService _userAddressService;
    private readonly IValidator<UserAddressRequestDto> _userAddressValidator;

    public UserAddressController(
        IUserAddressService userAddressService,
        IValidator<UserAddressRequestDto> userAddressValidator)
    {
        _userAddressService = userAddressService;
        _userAddressValidator = userAddressValidator;
    }

    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetUserAddress()
        => Ok(await _userAddressService.GetUserAddressAsync());

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> CreateUserAddress([FromBody] UserAddressRequestDto param)
    {
        ValidationHelper.ValidateResult(_userAddressValidator.Validate(param));
        return Ok(await _userAddressService.CreateUserAddressAsync(param));
    }

    [HttpPut("{userAddressId}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> UpdateUserAddress(Guid userAddressId, [FromBody] UserAddressRequestDto param)
    {
        ValidationHelper.ValidateResult(_userAddressValidator.Validate(param));
        await _userAddressService.UpdateUserAddressAsync(param, userAddressId);
        return NoContent();
    }

    [HttpPut("{userAddressId}/default")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> SetDefaultUserAddress(Guid userAddressId)
    {
        await _userAddressService.SetDefaultUserAddressAsync(userAddressId);
        return NoContent();
    }

    [HttpDelete("{userAddressId}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> DeleteUserAddress(Guid userAddressId)
    {
        await _userAddressService.DeleteUserAddressAsync(userAddressId);
        return NoContent();
    }
}
