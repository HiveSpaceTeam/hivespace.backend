using FluentValidation;
using HiveSpace.Application.Models.Dtos.Request.User;

namespace HiveSpace.Application.Validators.User;

public class CreateUserValidator : AbstractValidator<CreateUserRequestDto>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password length is at least 8 characters")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d]{8,}$").WithMessage("Password is not in correct format");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required")
            .Matches(@"^84(?:3[2-9]|5[6|8|9]|7[0|6-9]|8[1-9]|9[0-9])\d{7}$").WithMessage("Phone number is not in correct format");

        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("User name is required");
    }
}
