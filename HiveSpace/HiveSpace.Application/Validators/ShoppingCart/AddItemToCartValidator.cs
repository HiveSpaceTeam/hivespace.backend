using FluentValidation;
using HiveSpace.Application.Models.Dtos.Request.ShoppingCart;
using HiveSpace.Common.Exceptions.Models;
using HiveSpace.Domain.Enums;

namespace HiveSpace.Application.Validators.ShoppingCart;

public class AddItemToCartValidator : AbstractValidator<AddItemToCartRequestDto>
{
    public AddItemToCartValidator()
    {
        RuleFor(x => x.SkuId)
            .NotEmpty().WithState(x => new ErrorCode { Code = ApplicationErrorCode.Required, Source = nameof(x.SkuId) });

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithState(x => new ErrorCode { Code = ApplicationErrorCode.InvalidValue, Source = nameof(x.Quantity) });

        RuleFor(x => x.IsSelected)
            .NotEmpty().WithState(x => new ErrorCode { Code = ApplicationErrorCode.Required, Source = nameof(x.IsSelected) });
    }
}