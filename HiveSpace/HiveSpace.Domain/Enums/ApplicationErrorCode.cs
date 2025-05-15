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
    CategoryNotFound = 422012,
    ShoppingCartNotFound = 422013,
    CacheNotFound = 422014,
}
