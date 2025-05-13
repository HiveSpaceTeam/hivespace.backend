using FluentValidation;
using HiveSpace.Application.Models.Dtos.Request.CartItem;

namespace HiveSpace.Application.Validators.ShoppingCart;

public class UpdateCartItemValidator : AbstractValidator<UpdateCartItemRequestDto>
{
    public UpdateCartItemValidator()
    {
        RuleFor(x => x.CartId)
            .NotEmpty().WithMessage("CartId is required");

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0");
    }
}