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
    private readonly IConfiguration _configuration;
    private readonly IValidator<CreateUserRequestDto> _createUserValidator;
    private readonly IValidator<LoginRequestDto> _loginValidator;
    private readonly IValidator<ChangePasswordRequestDto> _changePasswordValidator;
    private readonly IValidator<UpdateUserRequestDto> _updateUserValidator;

    public UserController(
        IConfiguration configuration,
        IUserService userService,
        IValidator<CreateUserRequestDto> createUserValidator,
        IValidator<LoginRequestDto> loginValidator,
        IValidator<ChangePasswordRequestDto> changePasswordValidator,
        IValidator<UpdateUserRequestDto> updateUserValidator)
    {
        _userService = userService;
        _configuration = configuration;
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

        string connectionString = _configuration.GetSection("AzureBlobStorage:Connectionstring").Value ?? string.Empty;
         
        return Ok(connectionString);

        var validationResult = _createUserValidator.Validate(requestDto);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        var result = await _userService.CreateUserAsync(requestDto);
        return Ok(result);
    }

    [HttpPost("login")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto requestDto)
    {
        var validationResult = _loginValidator.Validate(requestDto);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        var result = await _userService.LoginAsync(requestDto);
        return Ok(result);
    }

    [Authorize]
    [HttpPut]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequestDto param)
    {
        var validationResult = _updateUserValidator.Validate(param);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
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
        var validationResult = _changePasswordValidator.Validate(requestDto);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        await _userService.ChangePassword(requestDto);
        return NoContent();
    }
}
