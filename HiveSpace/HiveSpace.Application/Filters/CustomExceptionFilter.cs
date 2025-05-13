using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using HiveSpace.Domain.Exceptions;
using HiveSpace.Common.Exceptions;
using Serilog;

namespace HiveSpace.Application.Filters
{
    public class CustomExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {

            context.ExceptionHandled = true;
            var errorResponse = new ErrorModel
            {
                Errors = [],
                Status = "500",
                Timestamp = DateTime.UtcNow,
                TraceId = Guid.NewGuid().ToString(),
                Version = "1.0"
            };

            if (context.Exception is BaseException exception)
            {
                var errors = exception.ErrorCodeList.Select(x => new ErrorCodeDto
                {
                    Code = x.Code == null ? "000000" : Convert.ToInt32(x.Code).ToString(),
                    Source = x.Source ?? (x.Data is not null && x.Data.Count > 0 ? x.Data[0].Key :  null),
                });
                errorResponse.Errors = errors.ToList();
                errorResponse.Status = exception.HttpCode.ToString();
            }

            context.Result = new ObjectResult(errorResponse)
            {
                StatusCode = int.Parse(errorResponse.Status),
            };
        }
    }
}
