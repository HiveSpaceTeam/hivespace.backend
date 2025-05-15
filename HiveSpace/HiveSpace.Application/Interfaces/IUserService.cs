using HiveSpace.Application.Models.Dtos.Request.User;
using HiveSpace.Application.Models.Dtos.Response.User;

namespace HiveSpace.Application.Interfaces;

public interface IUserService
{
    Task<SignupResponseDto> CreateUserAsync(CreateUserRequestDto requestDto);
    Task<LoginResponseDto> LoginAsync(LoginRequestDto requestDto);
    Task UpdateUserInfoAsync(UpdateUserRequestDto param);
    Task<UserInfoDto> GetUserInfoAsync();
    Task ChangePassword(ChangePasswordRequestDto requestDto);
}
