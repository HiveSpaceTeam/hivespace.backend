using HiveSpace.Domain.Enums;
using HiveSpace.Domain.Exceptions;
using HiveSpace.Domain.SeedWork;
using HiveSpace.Domain.Shared;

namespace HiveSpace.Domain.AggergateModels.UserAggregate;
public class User : AggregateRoot<Guid>
{
    public string? FullName { get; private set; }

    public string UserName { get; private set; }

    public string? Email { get; private set; }

    public PhoneNumber PhoneNumber { get; private set; }

    public string PasswordHashed { get; private set; }

    public Gender? Gender { get; private set; }
    public DateTime? DateOfBirth { get; private set; }

    private readonly List<UserAddress> _addresses = [];
    public IReadOnlyCollection<UserAddress> Addresses => _addresses.AsReadOnly();

    private User() { }

    public User(
        string phoneNumber,
        string password,
        string userName,
        string? fullName,
        string? email,
        Gender? gender = null,
        DateTime? dob = null)
    {
        PhoneNumber = new PhoneNumber(phoneNumber);
        PasswordHashed = password;
        UserName = userName;
        FullName = fullName;
        Email = email;
        Gender = gender;
        DateOfBirth = dob;
    }

    public UserAddress AddAddress(UserAddressProps props)
    {
        var address = new UserAddress(props);
        _addresses.Add(address);
        return address;
    }

    public void RemoveAddress(Guid userAddressId)
    {
        var deletedAddressIndex = _addresses.FindIndex(x => x.Id == userAddressId);
        if (deletedAddressIndex == -1) throw new NotFoundException("Address not found");
        _addresses.RemoveAt(deletedAddressIndex);
    }

    public void UpdateAddress(Guid userAddressId, UserAddressProps props)
    {
        var address = _addresses.Find(x => x.Id == userAddressId) ?? throw new NotFoundException("Address not found");
        address.UpdateAddress(props);
    }

    public void UpdatePassword(string passwordHashed)
    {
        PasswordHashed = passwordHashed;
    }

    public void UpdateUserInfo(string userName, string? fullName, string? email, string? phoneNumber, Gender? gender, DateTime? dob)
    {
        if (!string.IsNullOrWhiteSpace(userName)) UserName = userName;
        if (!string.IsNullOrWhiteSpace(fullName)) FullName = fullName;
        if (!string.IsNullOrWhiteSpace(email)) Email = email;
        if (!string.IsNullOrWhiteSpace(phoneNumber) && PhoneNumber.Value != phoneNumber) PhoneNumber = new PhoneNumber(phoneNumber);
        if (gender is not null) Gender = (Gender)gender;
        if (!string.IsNullOrWhiteSpace(email)) DateOfBirth = dob;
    }

    public void SetDefaultAddress(Guid userAddressId)
    {
        var address = _addresses.Find(x => x.Id == userAddressId) ?? throw new NotFoundException("Address not found");
        _addresses.ForEach(x => x.SetDefault(false));
        address.SetDefault(true);
    }
}
