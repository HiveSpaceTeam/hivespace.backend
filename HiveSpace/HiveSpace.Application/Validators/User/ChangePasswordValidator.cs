using FluentValidation;
using HiveSpace.Application.Models.Dtos.Request.User;

namespace HiveSpace.Application.Validators.User;

public class ChangePasswordValidator : AbstractValidator<ChangePasswordRequestDto>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Current password is required")
            .MinimumLength(8).WithMessage("Password length is at least 8 characters")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d]{8,}$").WithMessage("Password is not in correct format");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required")
            .MinimumLength(8).WithMessage("Password length is at least 8 characters")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d]{8,}$").WithMessage("Password is not in correct format")
            .NotEqual(x => x.Password).WithMessage("New password must be different from current password");
    }
}