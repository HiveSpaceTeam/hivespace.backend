using HiveSpace.Domain.SeedWork;
using HiveSpace.Domain.Shared;

namespace HiveSpace.Domain.AggergateModels.OrderAggregate;

public class ShippingAddress : ValueObject
{
    public string FullName { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public string OtherDetails { get; private set; }
    public Address Address { get; private set; }

    public ShippingAddress(string fullName, string phoneNumber, string otherDetails, string street, string ward, string district, string province, string country)
    {
        FullName = fullName;
        PhoneNumber = new PhoneNumber(phoneNumber);
        OtherDetails = otherDetails;
        Address = new Address(street, ward, district, province, country);
    }

    private ShippingAddress() { } // For ORM frameworks

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FullName;
        yield return PhoneNumber;
        yield return OtherDetails;
        yield return Address;
    }
}

public class Address : ValueObject
{
    public string Street { get; private set; }
    public string Ward { get; private set; }
    public string District { get; private set; }
    public string Province { get; private set; }
    public string Country { get; private set; }

    public Address(string street, string ward, string district, string province, string country)
    {
        Street = street;
        Ward = ward;
        District = district;
        Province = province;
        Country = country;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return Ward;
        yield return District;
        yield return Province;
        yield return Country;
    }
}

public class ShippingAddressProps
{
    public string FullName { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public string OtherDetails { get; set; } = default!;
    public string Street { get; set; } = default!;
    public string Ward { get; set; } = default!;
    public string District { get; set; } = default!;
    public string Province { get; set; } = default!;
    public string Country { get; set; } = default!;
}
