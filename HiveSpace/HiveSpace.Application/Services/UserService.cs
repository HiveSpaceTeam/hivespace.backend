using AutoMapper;
using HiveSpace.Application.Helpers;
using HiveSpace.Application.Interfaces;
using HiveSpace.Application.Models.Dtos.Request.User;
using HiveSpace.Application.Models.Dtos.Response.User;
using HiveSpace.Common.Interface;
using HiveSpace.Common.Models;
using HiveSpace.Domain.AggergateModels.UserAggregate;
using HiveSpace.Domain.Enums;
using HiveSpace.Domain.Repositories;

namespace HiveSpace.Application.Services;

public class UserService : IUserService
{
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IUserContext _userContext;

    public UserService(
        IUserRepository userRepository,
        IJwtService jwtService,
        IUserContext userContext,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _userContext = userContext;
        _mapper = mapper;
    }

    public async Task<SignupResponseDto> CreateUserAsync(CreateUserRequestDto requestDto)
    {
        if (await _userRepository.FindUserByPhoneNumber(requestDto.PhoneNumber) is not null)
        {
            throw ExceptionHelper.DomainException(ApplicationErrorCode.PhoneNumberExisted, nameof(User.PhoneNumber), requestDto.PhoneNumber);
        }

        var passwordHashed = PasswordHelper.Hash(requestDto.Password);
        var newUser = new User(
            requestDto.PhoneNumber,
            passwordHashed,
            requestDto.UserName,
            null,
            null,
            null,
            null);

        _userRepository.Add(newUser);
        await _userRepository.SaveChangesAsync();

        var identity = new Identity
        {
            UserId = newUser.Id,
            PhoneNumber = newUser.PhoneNumber.Value,
            Email = newUser.Email ?? string.Empty,
        };

        var contextData = new ContextData
        {
            Username = newUser.UserName,
            FullName = newUser.FullName,
        };

        return new SignupResponseDto
        {
            Token = _jwtService.GenerateToken(identity),
            ContextData = contextData
        };
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto requestDto)
    {
        var user = await _userRepository.FindUserByEmail(requestDto.Email)
            ?? throw ExceptionHelper.NotFoundException(ApplicationErrorCode.UserNotFound, nameof(User.Email), requestDto.Email);

        if (!PasswordHelper.Verify(requestDto.Password, user.PasswordHashed))
        {
            throw ExceptionHelper.DomainException(ApplicationErrorCode.IncorrectPassword);
        }

        var identity = new Identity
        {
            UserId = user.Id,
            PhoneNumber = user.PhoneNumber.Value,
            Email = user.Email ?? string.Empty,
        };

        var contextData = new ContextData
        {
            Username = user.UserName,
            FullName = user.FullName,
        };

        return new LoginResponseDto
        {
            Token = _jwtService.GenerateToken(identity),
            ContextData = contextData
        };
    }

    public async Task UpdateUserInfoAsync(UpdateUserRequestDto param)
    {
        var userId = _userContext.UserId;
        var user = await _userRepository.GetByIdAsync(userId, includeDetail: true)
            ?? throw ExceptionHelper.NotFoundException(ApplicationErrorCode.UserNotFound);

        user.UpdateUserInfo(param.UserName, param.FullName, param.Email, param.PhoneNumber, param.Gender, param.DateOfBirth);
        await _userRepository.SaveChangesAsync();
    }

    public async Task<UserInfoDto> GetUserInfoAsync()
    {
        var userId = _userContext.UserId;
        var user = await _userRepository.GetByIdAsync(userId, includeDetail: true)
            ?? throw ExceptionHelper.NotFoundException(ApplicationErrorCode.UserNotFound);
        return _mapper.Map<UserInfoDto>(user);
    }

    public async Task ChangePassword(ChangePasswordRequestDto requestDto)
    {
        var userId = _userContext.UserId;
        var user = await _userRepository.GetByIdAsync(userId, includeDetail: true)
            ?? throw ExceptionHelper.NotFoundException(ApplicationErrorCode.UserNotFound);

        if (!PasswordHelper.Verify(requestDto.Password, user.PasswordHashed))
        {
            throw ExceptionHelper.DomainException(ApplicationErrorCode.IncorrectPassword, nameof(user.PasswordHashed), requestDto.Password);
        }

        user.UpdatePassword(PasswordHelper.Hash(requestDto.NewPassword));
        await _userRepository.SaveChangesAsync();
    }
}
