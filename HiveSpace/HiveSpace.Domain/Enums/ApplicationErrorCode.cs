namespace HiveSpace.Domain.Enums;

public enum ApplicationErrorCode
{
    PhoneNumberExisted = 422001,
    InvalidSku = 422002,
    InvalidMoney = 422003,
    InvalidPhoneNumber = 422004,
    IncorrectPassword = 422005,
    UserNotFound = 422006,
    Unauthorized = 422007,
    UserAddressNotFound = 422008,
    InvalidQuantity = 422009,
    InvalidQuantitySku = 422010,
    FluentValidationError = 422011,
    FileNotFound = 422012,
    NoFileUploaded = 422013,
    Required = 422014,
    InvalidFormat = 422015,
    MinLengthNotMet = 422016,
    SimilarCurrentPassword = 422017,
    InvalidDate = 422018,
    InvalidValue = 422019,
    MaxLengthExceeded = 422020,
}
