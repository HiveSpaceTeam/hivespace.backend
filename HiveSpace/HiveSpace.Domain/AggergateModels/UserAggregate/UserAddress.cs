﻿using HiveSpace.Domain.Seedwork;
using HiveSpace.Domain.SeedWork;
using HiveSpace.Domain.Shared;

namespace HiveSpace.Domain.AggergateModels.UserAggregate;

public sealed class UserAddress : Entity<Guid>, IAuditable
{
    public string FullName { get; private set; }
    public string Street { get; private set; }
    public string Ward { get; private set; }
    public string District { get; private set; }
    public string Province { get; private set; }
    public string Country { get; private set; }
    public string? ZipCode { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public bool IsDefault { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    private UserAddress() { }

    public UserAddress(UserAddressProps props)
    {
        UpdateAddress(props);
    }

    internal void SetDefault(bool isDefault) => IsDefault = isDefault;

    public void UpdateAddress(UserAddressProps props)
    {
        FullName = props.FullName;
        Street = props.Street;
        Ward = props.Ward;
        District = props.District;
        Province = props.Province;
        Country = props.Country;
        ZipCode = props.ZipCode;
        PhoneNumber = new PhoneNumber(props.PhoneNumber);
    }
}

public class UserAddressProps
{
    public string FullName { get; set; } = default!;
    public string Street { get; set; } = default!;
    public string Ward { get; set; } = default!;
    public string District { get; set; } = default!;
    public string Province { get; set; } = default!;
    public string Country { get; set; } = default!;
    public string? ZipCode { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
}
