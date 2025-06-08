using FluentValidation;
using HiveSpace.Application.Models.Dtos.Request.User;
using HiveSpace.Common.Exceptions.Models;
using HiveSpace.Domain.Enums;

namespace HiveSpace.Application.Validators.User;

public class UpdateUserValidator : AbstractValidator<UpdateUserRequestDto>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithState(x => new ErrorCode { Code = ApplicationErrorCode.Required, Source = nameof(x.FullName) })
            .MaximumLength(50).WithState(x => new ErrorCode { Code = ApplicationErrorCode.MaxLengthExceeded, Source = nameof(x.FullName) })
            .When(x => x.FullName != null);

        RuleFor(x => x.UserName)
            .NotEmpty().WithState(x => new ErrorCode { Code = ApplicationErrorCode.Required, Source = nameof(x.UserName) })
            .MaximumLength(50).WithState(x => new ErrorCode { Code = ApplicationErrorCode.MaxLengthExceeded, Source = nameof(x.UserName) })
            .When(x => x.UserName != null);

        RuleFor(x => x.Email)
            .EmailAddress().WithState(x => new ErrorCode { Code = ApplicationErrorCode.InvalidFormat, Source = nameof(x.Email) })
            .MaximumLength(100).WithState(x => new ErrorCode { Code = ApplicationErrorCode.MaxLengthExceeded, Source = nameof(x.Email) })
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithState(x => new ErrorCode { Code = ApplicationErrorCode.Required, Source = nameof(x.PhoneNumber) })
            .When(x => x.PhoneNumber != null);

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithState(x => new ErrorCode { Code = ApplicationErrorCode.Required, Source = nameof(x.DateOfBirth) })
            .LessThan(DateTime.Now).WithState(x => new ErrorCode { Code = ApplicationErrorCode.InvalidDate, Source = nameof(x.DateOfBirth) })
            .When(x => x.DateOfBirth != null);

        RuleFor(x => x.Gender)
            .Must(gender => Enum.IsDefined(typeof(Gender), gender!))
            .WithState(x => new ErrorCode { Code = ApplicationErrorCode.InvalidValue, Source = nameof(x.Gender) })
            .When(x => x.Gender != null);
    }
}
