using FluentValidation;
using HiveSpace.Application.Models.Dtos.Request.Paging;

namespace HiveSpace.Application.Validators.Paging;

public class PagingValidator : AbstractValidator<PagingRequestDto>
{
    public PagingValidator()
    {
        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("PageSize must be greater than 0");

        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(0).WithMessage("PageNumber must be greater than or equal to 0");
    }
}