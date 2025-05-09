using FluentValidation;
using HiveSpace.Application.Models.Dtos.Request.ShoppingCart;

namespace HiveSpace.Application.Validators.ShoppingCart;

public class UpdateMultiCartItemSelectionValidator : AbstractValidator<UpdateMultiCartItemSelectionDto>
{
    public UpdateMultiCartItemSelectionValidator()
    {
        RuleFor(x => x.SkuIds)
            .NotEmpty().WithMessage("SkuIds is required")
            .Must(x => x.Count > 0).WithMessage("At least one SkuId is required");

        RuleFor(x => x.CartId)
            .NotNull().WithMessage("CartId is required");

        RuleFor(x => x.IsSelected)
            .NotNull().WithMessage("IsSelected is required");
    }
}