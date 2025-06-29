using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using HiveSpace.Common.Exceptions;
using HiveSpace.Common.Exceptions.Models;
using HiveSpace.Domain.Exceptions;

namespace HiveSpace.Common.Filters
{
    public class CustomExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {

            context.ExceptionHandled = true;
            var errorResponse = new ExceptionModel
            {
                Errors = [],
                Status = "500",
                Timestamp = DateTime.UtcNow,
                TraceId = Guid.NewGuid().ToString(),
                Version = "1.0"
            };

            if (context.Exception is BaseException exception)
            {
                var errorList = new List<ErrorCodeDto>();
                foreach (var error in exception.ErrorCodeList)
                {
                    var errorDto = new ErrorCodeDto
                    {
                        Code = error.Code == null ? "000000" : Convert.ToInt32(error.Code).ToString(),
                        MessageCode = error.Code?.ToString() ?? string.Empty,
                        Source = error.Source ?? (error.Data is not null && error.Data.Count > 0 ? error.Data[0].Key : null),
                    };
                    if (exception.EnableData && error.Data is not null)
                    {
                        foreach (var item in error.Data)
                        {
                            errorDto.Data.Add(item.Key, item.Value);
                        }
                    }
                    errorList.Add(errorDto);
                }
                errorResponse.Errors = [.. errorList];
                errorResponse.Status = exception.HttpCode.ToString();
            }
            else if (context.Exception is DomainException domainException)
            {
                var errorCode = domainException.ErrorCode;
                var error = new ErrorCodeDto
                {
                    Code = errorCode == null ? "000000" : Convert.ToInt32(errorCode).ToString(),
                    MessageCode = errorCode?.ToString() ?? string.Empty,
                    Source = domainException.Key
                };
            }
            else
            {
                var error = new ErrorCodeDto
                {
                    Code = "000000",
                    MessageCode = "ServerError",
                    Source = null,
                };
#if DEBUG
                error.MessageCode = context.Exception.Message;
#endif
                errorResponse.Errors = [error];
            }

            context.Result = new ObjectResult(errorResponse)
            {
                StatusCode = int.Parse(errorResponse.Status),
            };
        }
    }
}
