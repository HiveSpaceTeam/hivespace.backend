using FluentValidation.Results;
using HiveSpace.Common.Exceptions;
using HiveSpace.Domain.Enums;

namespace HiveSpace.Application.Helpers;

public static class ValidationHelper
{
    public static void Validate(ValidationResult validationResult)
    {
        if (!validationResult.IsValid)
        {
            throw ExceptionHelper.BadRequestException(ApplicationErrorCode.FluentValidationError, validationResult.Errors);
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
