using FluentValidation;
using HiveSpace.Application.Constants;
using HiveSpace.Application.Models.Dtos.Request.User;
using HiveSpace.Common.Exceptions;
using HiveSpace.Common.Exceptions.Models;
using HiveSpace.Domain.Enums;

namespace HiveSpace.Application.Validators.User;

public class ChangePasswordValidator : AbstractValidator<ChangePasswordRequestDto>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.Password)
            .NotEmpty().WithState(x => new ErrorCode { Code = ApplicationErrorCode.Required, Source = nameof(x.Password) })
            .MinimumLength(8).WithState(x => new ErrorCode { Code = ApplicationErrorCode.MinLengthNotMet, Source = nameof(x.Password) })
            .Matches(ApplicationConstant.PasswordFormat).WithState(x => new ErrorCode { Code = ApplicationErrorCode.InvalidFormat, Source = nameof(x.Password) });

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithState(x => new ErrorCode { Code = ApplicationErrorCode.Required, Source = nameof(x.NewPassword) })
            .MinimumLength(8).WithState(x => new ErrorCode { Code = ApplicationErrorCode.MinLengthNotMet, Source = nameof(x.NewPassword) })
            .Matches(ApplicationConstant.PasswordFormat).WithState(x => new ErrorCode { Code = ApplicationErrorCode.InvalidFormat, Source = nameof(x.NewPassword) })
            .NotEqual(x => x.Password).WithState(x => new ErrorCode { Code = ApplicationErrorCode.SimilarCurrentPassword, Source = nameof(x.NewPassword) });
    }
}