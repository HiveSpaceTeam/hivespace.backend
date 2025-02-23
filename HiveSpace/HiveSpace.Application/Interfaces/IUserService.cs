using HiveSpace.Application.Models.Dtos.Request.User;
using HiveSpace.Application.Models.Dtos.Response.User;

namespace HiveSpace.Application.Interfaces;

public interface IUserService
{
    Task<Guid> CreateUserAsync(CreateUserRequestDto requestDto);
    Task<LoginResponseDto> LoginAsync(LoginRequestDto requestDto);
    Task<bool> UpdateUserInfoAsync(UpdateUserRequestDto param);
    Task<UserInfoDto> GetUserInfoAsync();
    Task<bool> ChangePassword(ChangePasswordRequestDto requestDto);
}
