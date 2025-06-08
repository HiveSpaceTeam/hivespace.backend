using HiveSpace.Common.Exceptions.Models;

namespace HiveSpace.Common.Exceptions;
public class UnauthorizedException : BaseException
{
    private static readonly int _httpCode = 401;

    public UnauthorizedException(List<ErrorCode> errorCodeList, bool? enableData = false)
        : base(errorCodeList, _httpCode, enableData)
    {
    }

    public UnauthorizedException(List<ErrorCode> errorCodeList, Exception inner, bool? enableData = false)
        : base(errorCodeList, inner, _httpCode, enableData)
    {
    }
}
