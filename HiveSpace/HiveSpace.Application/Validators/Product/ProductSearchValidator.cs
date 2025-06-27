using FluentValidation;
using HiveSpace.Application.Models.Dtos.Request.Product;
using HiveSpace.Common.Exceptions.Models;
using HiveSpace.Domain.Enums;

namespace HiveSpace.Application.Validators.Product;

public class ProductSearchValidator : AbstractValidator<ProductSearchRequestDto>
{
    public ProductSearchValidator()
    {
        RuleFor(x => x.Keyword)
            .NotEmpty().WithState(x => new ErrorCode { Code = ApplicationErrorCode.Required, Source = nameof(x.Keyword) });

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithState(x => new ErrorCode { Code = ApplicationErrorCode.MustBeGreaterThanZero, Source = nameof(x.PageSize) });

        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithState(x => new ErrorCode { Code = ApplicationErrorCode.MustBeGreaterThanZero, Source = nameof(x.PageNumber) });
    }
}