using FluentValidation;
using HiveSpace.Application.Models.Dtos.Request.ShoppingCart;
using HiveSpace.Common.Exceptions.Models;
using HiveSpace.Domain.Enums;

namespace HiveSpace.Application.Validators.ShoppingCart;

public class UpdateMultiCartItemSelectionValidator : AbstractValidator<UpdateMultiCartItemSelectionDto>
{
    public UpdateMultiCartItemSelectionValidator()
    {
        RuleFor(x => x.SkuIds)
            .NotEmpty().WithState(x => new ErrorCode { Code = ApplicationErrorCode.Required, Source = nameof(x.SkuIds) })
            .Must(x => x.Count > 0).WithState(x => new ErrorCode { Code = ApplicationErrorCode.InvalidValue, Source = nameof(x.SkuIds) });

        RuleFor(x => x.CartId)
            .NotNull().WithState(x => new ErrorCode { Code = ApplicationErrorCode.Required, Source = nameof(x.CartId) });

        RuleFor(x => x.IsSelected)
            .NotNull().WithState(x => new ErrorCode { Code = ApplicationErrorCode.Required, Source = nameof(x.IsSelected) });
    }
}