using FluentValidation;
using HiveSpace.Application.Helpers;
using HiveSpace.Application.Interfaces;
using HiveSpace.Application.Models.Dtos.Request.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HiveSpace.Application.Controllers;

[Route("api/v1/users")]
[ApiController]
public class UserController : Controller
{
    private readonly IUserService _userService;
    private readonly IValidator<CreateUserRequestDto> _createUserValidator;
    private readonly IValidator<LoginRequestDto> _loginValidator;
    private readonly IValidator<ChangePasswordRequestDto> _changePasswordValidator;
    private readonly IValidator<UpdateUserRequestDto> _updateUserValidator;

    public UserController(
        IUserService userService,
        IValidator<CreateUserRequestDto> createUserValidator,
        IValidator<LoginRequestDto> loginValidator,
        IValidator<ChangePasswordRequestDto> changePasswordValidator,
        IValidator<UpdateUserRequestDto> updateUserValidator)
    {
        _userService = userService;
        _createUserValidator = createUserValidator;
        _loginValidator = loginValidator;
        _changePasswordValidator = changePasswordValidator;
        _updateUserValidator = updateUserValidator;
    }

    [HttpPost("signup")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Signup([FromBody] CreateUserRequestDto requestDto)
    {
        ValidationHelper.ValidateResult(_createUserValidator.Validate(requestDto));
        var result = await _userService.CreateUserAsync(requestDto);
        return Ok(result);
    }

    [HttpPost("login")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto requestDto)
    {
        ValidationHelper.ValidateResult(_loginValidator.Validate(requestDto));
        var result = await _userService.LoginAsync(requestDto);
        return Ok(result);
    }

    [Authorize]
    [HttpPut]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequestDto param)
    {
        ValidationHelper.ValidateResult(_updateUserValidator.Validate(param));
        await _userService.UpdateUserInfoAsync(param);
        return NoContent();
    }

    [Authorize]
    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetUserInfo()
    {
        var result = await _userService.GetUserInfoAsync();
        return Ok(result);
    }

    [HttpPut("change-password")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto requestDto)
    {
        ValidationHelper.ValidateResult(_changePasswordValidator.Validate(requestDto));
        await _userService.ChangePassword(requestDto);
        return NoContent();
    }
}
