using FluentValidation;
using HiveSpace.Application.Models.Dtos.Request.ShoppingCart;

namespace HiveSpace.Application.Validators.Order;

public class CreateOrderValidator : AbstractValidator<CreateOrderRequestDto>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.UserAddressID)
            .NotNull().WithMessage("UserAddressID is required");

        RuleFor(x => x.PaymentMethod)
            .NotEmpty().WithMessage("PaymentMethod is required");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Items is required");
    }
}