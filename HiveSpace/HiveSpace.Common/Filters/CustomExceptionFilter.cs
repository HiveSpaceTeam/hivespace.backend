using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using HiveSpace.Common.Exceptions;
using HiveSpace.Common.Exceptions.Models;

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
                //var errors = exception.ErrorCodeList.Select(x => new ErrorCodeDto
                //{
                //    Code = x.Code == null ? "000000" : Convert.ToInt32(x.Code).ToString(),
                //    MessageCode = x.Code?.ToString() ?? string.Empty,
                //    Data = x.Data == null ? [] : x.Data.ToDictionary(item => item.Key, item => item.Value),
                //    Source = x.Source ?? (x.Data is not null && x.Data.Count > 0 ? x.Data[0].Key : null),
                //});
                var errorList = new List<ErrorCodeDto>();
                foreach (var error in exception.ErrorCodeList)
                {
                    var errorDto = new ErrorCodeDto
                    {
                        Code = error.Code == null ? "000000" : Convert.ToInt32(error.Code).ToString(),
                        MessageCode = error.Code?.ToString() ?? string.Empty,
                        Source = error.Source ?? (error.Data is not null && error.Data.Count > 0 ? error.Data[0].Key : null),
                    };
                    if (exception.EnableData)
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
            else
            {
                var error = new ErrorCodeDto
                {
                    Code = "000000",
                    MessageCode = "ServerError",
                    Data = [],
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
