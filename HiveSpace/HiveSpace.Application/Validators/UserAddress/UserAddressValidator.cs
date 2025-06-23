using FluentValidation;
using HiveSpace.Application.Models.Dtos.Request.UserAddress;
using HiveSpace.Common.Exceptions.Models;
using HiveSpace.Domain.Enums;

namespace HiveSpace.Application.Validators.UserAddress;

public class UserAddressValidator : AbstractValidator<UserAddressRequestDto>
{
    public UserAddressValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithState(_ => new ErrorCode { Code = ApplicationErrorCode.Required, Source = nameof(UserAddressRequestDto.FullName) });

        RuleFor(x => x.Street)
            .NotEmpty()
            .WithState(_ => new ErrorCode { Code = ApplicationErrorCode.Required, Source = nameof(UserAddressRequestDto.Street) });

        RuleFor(x => x.Ward)
            .NotEmpty()
            .WithState(_ => new ErrorCode { Code = ApplicationErrorCode.Required, Source = nameof(UserAddressRequestDto.Ward) });

        RuleFor(x => x.District)
            .NotEmpty()
            .WithState(_ => new ErrorCode { Code = ApplicationErrorCode.Required, Source = nameof(UserAddressRequestDto.District) });

        RuleFor(x => x.Province)
            .NotEmpty()
            .WithState(_ => new ErrorCode { Code = ApplicationErrorCode.Required, Source = nameof(UserAddressRequestDto.Province) });

        RuleFor(x => x.Country)
            .NotEmpty()
            .WithState(_ => new ErrorCode { Code = ApplicationErrorCode.Required, Source = nameof(UserAddressRequestDto.Country) });

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithState(_ => new ErrorCode { Code = ApplicationErrorCode.Required, Source = nameof(UserAddressRequestDto.PhoneNumber) });
    }
}
