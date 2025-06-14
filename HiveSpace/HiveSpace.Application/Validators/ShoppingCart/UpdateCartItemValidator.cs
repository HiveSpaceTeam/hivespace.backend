using FluentValidation;
using HiveSpace.Application.Models.Dtos.Request.CartItem;
using HiveSpace.Common.Exceptions.Models;
using HiveSpace.Domain.Enums;

namespace HiveSpace.Application.Validators.ShoppingCart;

public class UpdateCartItemValidator : AbstractValidator<UpdateCartItemRequestDto>
{
    public UpdateCartItemValidator()
    {
        RuleFor(x => x.CartId)
            .NotEmpty().WithState(x => new ErrorCode { Code = ApplicationErrorCode.Required, Source = nameof(x.CartId) });

        RuleFor(x => x.Id)
            .NotEmpty().WithState(x => new ErrorCode { Code = ApplicationErrorCode.Required, Source = nameof(x.Id) });

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithState(x => new ErrorCode { Code = ApplicationErrorCode.InvalidValue, Source = nameof(x.Quantity) });
    }
}