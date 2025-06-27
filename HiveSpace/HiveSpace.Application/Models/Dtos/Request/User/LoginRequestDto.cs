namespace HiveSpace.Application.Models.Dtos.Request.User;

public class LoginRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
