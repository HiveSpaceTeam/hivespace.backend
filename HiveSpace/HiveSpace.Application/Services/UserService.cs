using AutoMapper;
using HiveSpace.Application.Helpers;
using HiveSpace.Application.Interfaces;
using HiveSpace.Application.Models.Dtos.Request.User;
using HiveSpace.Application.Models.Dtos.Response.User;
using HiveSpace.Common.Interface;
using HiveSpace.Common.Models;
using HiveSpace.Domain.AggergateModels.UserAggregate;
using HiveSpace.Domain.Enums;
using HiveSpace.Domain.Exceptions;
using HiveSpace.Domain.Repositories;

namespace HiveSpace.Application.Services;

public class UserService(IUserRepository userRepository, IJwtService jwtService, IUserContext userContext, IMapper mapper) : IUserService
{
    private readonly IMapper _mapper = mapper;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IJwtService _jwtService = jwtService;
    private readonly IUserContext _userContext = userContext;
    public async Task<Guid> CreateUserAsync(CreateUserRequestDto requestDto)
    {
        var user = await _userRepository.FindUserByPhoneNumber(requestDto.PhoneNumber);

        if (user is not null)
        {
            throw new DomainException
            {
                Errors =
                [
                    new() {
                        Field="PhoneNumber",
                        MessageCode="i18nUser.PhoneNumberExisted",
                        ErrorCode=ErrorCode.PhoneNumberExisted
                    }
                ]
            };
        }

        var passwordHashed = PasswordHelper.Hash(requestDto.Password);
        var newUser = new User(
            requestDto.PhoneNumber,
            passwordHashed,
            requestDto.UserName,
            null,
            null,
            null, null);

        _userRepository.Add(newUser);
        await _userRepository.SaveChangesAsync();

        return newUser.Id;
    }


    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto requestDto)
    {
        var user = await _userRepository.FindUserByPhoneNumber(requestDto.PhoneNumber) ?? throw new NotFoundException("i18nAuth.messages.notFoundPhoneNumber");

        var isVerified = PasswordHelper.Verify(requestDto.Password, user.PasswordHashed);

        if (!isVerified)
        {
            throw new UnauthorizedAccessException("i18nAuth.messages.passwordIncorrect");
        }

        var identity = new Identity
        {
            UserId = user.Id,
            PhoneNumber = user.PhoneNumber.Value,
            Email = user.Email ?? "",
        };

        ContextData contextData = new ContextData
        {
            Username = user.UserName,
            FullName = user.FullName,
        };
        return new LoginResponseDto { Token = _jwtService.GenerateToken(identity), ContextData = contextData };
    }

    public async Task<bool> UpdateUserInfoAsync(UpdateUserRequestDto param)
    {
        var user = await _userRepository.GetByIdAsync(_userContext.UserId, includeDetail: true) ?? throw new NotFoundException("i18nUser.messages.notFoundUser");

        user.UpdateUserInfo(param.UserName, param.FullName, param.Email, param.PhoneNumber, param.Gender, param.DateOfBirth);
        await _userRepository.SaveChangesAsync();
        return true;
    }

    public async Task<UserInfoDto> GetUserInfoAsync()
    {
        var user = await _userRepository.GetByIdAsync(_userContext.UserId, includeDetail: true) ?? throw new NotFoundException("i18nUser.messages.notFoundUser");
        var res = _mapper.Map<UserInfoDto>(user);
        return res;
    }

    public async Task<bool> ChangePassword(ChangePasswordRequestDto requestDto)
    {
        var user = await _userRepository.GetByIdAsync(_userContext.UserId, includeDetail: true) ?? throw new NotFoundException("i18nUser.messages.notFoundUser");

        var isVerified = PasswordHelper.Verify(requestDto.Password, user.PasswordHashed);

        if (!isVerified)
        {
            throw new DomainException
            {
                MessageCode = "i18nUser.messages.incorrectPassword"
            };
        }
        var passwordHashed = PasswordHelper.Hash(requestDto.NewPassword);
        user.UpdatePassword(passwordHashed);
        await _userRepository.SaveChangesAsync();
        return true;
    }
}
