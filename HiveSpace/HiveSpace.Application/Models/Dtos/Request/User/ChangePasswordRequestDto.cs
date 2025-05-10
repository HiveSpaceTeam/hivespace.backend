namespace HiveSpace.Application.Models.Dtos.Request.User;

public class ChangePasswordRequestDto
{
    public string Password { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
