using FluentValidation;
using HiveSpace.Application.Models.Dtos.Request.Product;

namespace HiveSpace.Application.Validators.Product;

public class ProductHomeValidator : AbstractValidator<ProductHomeRequestDto>
{
    public ProductHomeValidator()
    {
        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("PageSize must be greater than 0");

        RuleFor(x => x.PageIndex)
            .GreaterThanOrEqualTo(0).WithMessage("PageIndex must be greater than or equal to 0");
    }
}