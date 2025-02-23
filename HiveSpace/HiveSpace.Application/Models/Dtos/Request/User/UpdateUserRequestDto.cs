using HiveSpace.Domain.Enums;

namespace HiveSpace.Application.Models.Dtos.Request.User;

public class UpdateUserRequestDto
{
    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
}
