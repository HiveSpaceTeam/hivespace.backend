namespace HiveSpace.Application.Models.Dtos.Request.User;

public class LoginRequestDto
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
