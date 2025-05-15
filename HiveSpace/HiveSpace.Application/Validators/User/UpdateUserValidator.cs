using FluentValidation;
using HiveSpace.Application.Models.Dtos.Request.User;
using HiveSpace.Domain.Enums;

namespace HiveSpace.Application.Validators.User;

public class UpdateUserValidator : AbstractValidator<UpdateUserRequestDto>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required")
            .MaximumLength(50).WithMessage("Full name must not exceed 50 characters")
            .When(x => x.FullName != null);

        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("User name is required")
            .MaximumLength(50).WithMessage("User name must not exceed 50 characters")
            .When(x => x.UserName != null);

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(100).WithMessage("Email must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required")
            .When(x => x.PhoneNumber != null);

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required")
            .LessThan(DateTime.Now).WithMessage("Date of birth must be in the past")
            .When(x => x.DateOfBirth != null);

        RuleFor(x => x.Gender)
            .Must(gender => Enum.IsDefined(typeof(Gender), gender!))
            .WithMessage("Gender must be a valid value from the Gender enum")
            .When(x => x.Gender != null);
    }
}