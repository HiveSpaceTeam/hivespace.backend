using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using HiveSpace.Domain.Exceptions;
using Serilog;

namespace HiveSpace.Application.Filters
{
    public class CustomExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {

            context.ExceptionHandled = true;

            switch (context.Exception)
            {
                case NotFoundException notFoundException:
                    context.Result = new ObjectResult(notFoundException)
                    {
                        StatusCode = 404,
                        Value = notFoundException.Message
                    };
                    break;
                case UnauthorizedAccessException unauthorizedAccessException:
                    context.Result = new ObjectResult(unauthorizedAccessException)
                    {
                        StatusCode = 401,
                        Value = unauthorizedAccessException.Message
                    };
                    break;

                case DomainException domainException:
                    context.Result = new ObjectResult(domainException)
                    {
                        StatusCode = 422,
                        Value = domainException.ToString()
                    };
                    break;
                default:
                    string msg = "Exception";
#if DEBUG
                    msg = context.Exception.ToString();
#endif
                    context.Result = new ObjectResult(context.Exception)
                    {
                        StatusCode = 500,
                        Value = msg
                    };

                    Log.Error(context.Exception.ToString());
                    break;
            }
        }
    }
}
