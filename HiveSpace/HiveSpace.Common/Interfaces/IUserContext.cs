namespace HiveSpace.Common.Interface;
public interface IUserContext
{
    bool IsAuthenticated { get; }
    Guid UserId { get; }
    string PhoneNumber { get; }
    string Email { get; }
}
