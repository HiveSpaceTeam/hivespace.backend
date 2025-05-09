using FluentValidation;
using HiveSpace.Application.Models.Dtos.Request.ShoppingCart;

namespace HiveSpace.Application.Validators.ShoppingCart;

public class AddItemToCartValidator : AbstractValidator<AddItemToCartRequestDto>
{
    public AddItemToCartValidator()
    {
        RuleFor(x => x.SkuId)
            .NotEmpty().WithMessage("SkuId is required");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0");

        RuleFor(x => x.IsSelected)
            .NotEmpty().WithMessage("IsSelected is required");
    }
}