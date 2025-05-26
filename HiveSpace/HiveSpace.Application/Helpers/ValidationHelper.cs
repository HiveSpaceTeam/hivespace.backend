using FluentValidation.Results;
using HiveSpace.Common.Exceptions;
using HiveSpace.Common.Exceptions.Models;
using HiveSpace.Domain.Enums;

namespace HiveSpace.Application.Helpers;

public static class ValidationHelper
{
    public static void ValidateResult(IEnumerable<ValidationResult> validationResults)
    {
        var errors = validationResults.SelectMany(x => ValidateResultWithState(x));
        if (errors.Any())
        {
            throw new BadRequestException(errors.ToList());
        }
    }

    public static void ValidateResult(ValidationResult validationResult)
    {
        if (!validationResult.IsValid)
        {
            throw new BadRequestException(ValidateResultWithState(validationResult).ToList());
        }
    }

    public static IEnumerable<ErrorCode> ValidateResultWithState(ValidationResult validationResult)
    {
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.Select(x => x.CustomState as ErrorCode ?? new ErrorCode 
            { 
                Code = ApplicationErrorCode.FluentValidationError,
            }).ToList();
        }
        return [];
    }
}
