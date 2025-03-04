namespace HiveSpace.Application.Models.Dtos.Response.User;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public ContextData ContextData { get; set; }
}
