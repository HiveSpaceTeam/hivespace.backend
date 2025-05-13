using FluentValidation;
using HiveSpace.Application.Models.Dtos.Request.User;

namespace HiveSpace.Application.Validators.User;

public class UpdateUserValidator : AbstractValidator<UpdateUserRequestDto>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required")
            .MaximumLength(50).WithMessage("Full name must not exceed 50 characters");

        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("User name is required")
            .MaximumLength(50).WithMessage("User name must not exceed 50 characters");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(100).WithMessage("Email must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required")
            .Matches(@"^84(?:3[2-9]|5[6|8|9]|7[0|6-9]|8[1-9]|9[0-9])\d{7}$").WithMessage("Phone number is not in correct format");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required")
            .LessThan(DateTime.Now).WithMessage("Date of birth must be in the past");
    }
}