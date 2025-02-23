using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HiveSpace.Application.Interfaces;
using HiveSpace.Application.Models.Dtos.Request.UserAddress;
using HiveSpace.Common.Interface;
using System.Net;

namespace HiveSpace.Application.Controllers;

[Route("api/v1/users/address")]
[ApiController]
[Authorize]
public class UserAddressController : Controller
{
    private readonly IUserAddressService _userAddressService;
    private readonly IValidator<UserAddressRequestDto> _userAddressValidator;

    public UserAddressController(IUserAddressService userAddressService, IValidator<UserAddressRequestDto> userAddressValidator)
    {
        _userAddressService = userAddressService;
        _userAddressValidator = userAddressValidator;
    }

    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetUserAddress()
    {
        var result = await _userAddressService.GetUserAddressAsync();
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> CreateUserAddress([FromBody] UserAddressRequestDto param)
    {
        _userAddressValidator.Validate(param);
        var result = await _userAddressService.CreateUserAddressAsync(param);
        return Ok(result);
    }

    [HttpPut("{userAddressId}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> UpdateUserAddress(Guid userAddressId, [FromBody] UserAddressRequestDto param)
    {
        _userAddressValidator.Validate(param);
        var result = await _userAddressService.UpdateUserAddressAsync(param, userAddressId);
        return Ok(result);
    }

    [HttpPut("{userAddressId}/default")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> SetDefaultUserAddress(Guid userAddressId)
    {
        var result = await _userAddressService.SetDefaultUserAddressAsync(userAddressId);
        return Ok(result);
    }

    [HttpDelete("{userAddressId}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> DeleteUserAddress(Guid userAddressId)
    {
        var result = await _userAddressService.DeleteUserAddressAsync(userAddressId);
        return Ok(result);
    }
}
