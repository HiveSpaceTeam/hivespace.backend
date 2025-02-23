using HiveSpace.Domain.Enums;

namespace HiveSpace.Application.Models.Dtos.Request.User
{
    public class UserInfoDto
    {
        public string? FullName { get; set; }
        public string UserName { get; set; }
        public string? Email { get; set; }
        public string PhoneNumber { get; set; }
        public Gender? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}
