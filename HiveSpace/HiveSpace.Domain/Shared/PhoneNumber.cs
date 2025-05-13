using HiveSpace.Domain.Enums;
using HiveSpace.Domain.Exceptions;
using HiveSpace.Domain.SeedWork;
using System.Text.RegularExpressions;

namespace HiveSpace.Domain.Shared;
public partial class PhoneNumber : ValueObject
{
    public string Value { get; private set; }
    public Regex regex = PhoneNumberRegex();

    private PhoneNumber() { }

    public PhoneNumber(string value)
    {
        Value = value;
        ValidateValueObject();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    protected void ValidateValueObject()
    {
        if (!regex.IsMatch(Value))
        {
            throw new DomainException
            {
                Errors =
                [
                    new() {
                        Field="PhoneNumber",
                        MessageCode="i18nPhoneNumber.InvalidPhoneNumber",
                        ErrorCode=ApplicationErrorCode.InvalidPhoneNumber
                    }
                ]
            };
        }
    }

    [GeneratedRegex(@"^84(?:3[2-9]|5[6|8|9]|7[0|6-9]|8[1-9]|9[0-9])\d{7}$")]
    private static partial Regex PhoneNumberRegex();
}
