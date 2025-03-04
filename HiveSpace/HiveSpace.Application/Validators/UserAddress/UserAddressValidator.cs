using FluentValidation;
using HiveSpace.Application.Models.Dtos.Request.UserAddress;

namespace HiveSpace.Application.Validators.UserAddress;

public class UserAddressValidator : AbstractValidator<UserAddressRequestDto>
{
    public UserAddressValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("FullName is required");
        
        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("Street is required");

        RuleFor(x => x.Ward)
            .NotEmpty().WithMessage("Ward is required");

        RuleFor(x => x.District)
            .NotEmpty().WithMessage("District is required");

        RuleFor(x => x.Province)
            .NotEmpty().WithMessage("Province is required");

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country is required");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("PhoneNumber is required");
    }
}
