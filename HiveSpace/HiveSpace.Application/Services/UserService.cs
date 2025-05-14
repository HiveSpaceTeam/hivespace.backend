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

public class UserService(IUserRepository userRepository, IJwtService jwtService, IUserContext userContext, IMapper mapper) : IUserService
{
    private readonly IMapper _mapper = mapper;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IJwtService _jwtService = jwtService;
    private readonly IUserContext _userContext = userContext;
    public async Task<SignupResponseDto> CreateUserAsync(CreateUserRequestDto requestDto)
    {
        var user = await _userRepository.FindUserByPhoneNumber(requestDto.PhoneNumber);

        if (user is not null)
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
            Email = newUser.Email ?? "",
        };

        ContextData contextData = new()
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
        var user = await _userRepository.FindUserByPhoneNumber(requestDto.PhoneNumber) 
            ?? throw ExceptionHelper.NotFoundException(ApplicationErrorCode.UserNotFound, nameof(User.PhoneNumber), requestDto.PhoneNumber);

        var isVerified = PasswordHelper.Verify(requestDto.Password, user.PasswordHashed);

        if (!isVerified)
        {
            throw ExceptionHelper.DomainException(ApplicationErrorCode.IncorrectPassword);
        }

        var identity = new Identity
        {
            UserId = user.Id,
            PhoneNumber = user.PhoneNumber.Value,
            Email = user.Email ?? "",
        };

        ContextData contextData = new ()
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
        var user = await _userRepository.GetByIdAsync(_userContext.UserId, includeDetail: true)
            ?? throw ExceptionHelper.NotFoundException(ApplicationErrorCode.UserNotFound);

        user.UpdateUserInfo(param.UserName, param.FullName, param.Email, param.PhoneNumber, param.Gender, param.DateOfBirth);
        await _userRepository.SaveChangesAsync();
    }

    public async Task<UserInfoDto> GetUserInfoAsync()
    {
        var user = await _userRepository.GetByIdAsync(_userContext.UserId, includeDetail: true)
            ?? throw ExceptionHelper.NotFoundException(ApplicationErrorCode.UserNotFound);
        var res = _mapper.Map<UserInfoDto>(user);
        return res;
    }

    public async Task ChangePassword(ChangePasswordRequestDto requestDto)
    {
        var user = await _userRepository.GetByIdAsync(_userContext.UserId, includeDetail: true)
            ?? throw ExceptionHelper.NotFoundException(ApplicationErrorCode.UserNotFound);

        var isVerified = PasswordHelper.Verify(requestDto.Password, user.PasswordHashed);

        if (!isVerified)
        {
            throw ExceptionHelper.DomainException(ApplicationErrorCode.IncorrectPassword, nameof(user.PasswordHashed), requestDto.Password);
        }
        var passwordHashed = PasswordHelper.Hash(requestDto.NewPassword);
        user.UpdatePassword(passwordHashed);
        await _userRepository.SaveChangesAsync();
    }
}
