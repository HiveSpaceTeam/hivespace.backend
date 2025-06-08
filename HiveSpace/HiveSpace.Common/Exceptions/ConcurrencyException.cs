using HiveSpace.Common.Exceptions.Models;

namespace HiveSpace.Common.Exceptions;
public class ConcurrencyException : BaseException
{
    private static readonly int _httpCode = 409; // HTTP 409 Conflict

    public ConcurrencyException(List<ErrorCode> errorCodeList, bool? enableData = false)
        : base(errorCodeList, _httpCode, enableData)
    {
    }

    public ConcurrencyException(List<ErrorCode> errorCodeList, Exception inner, bool? enableData = false)
        : base(errorCodeList, inner, _httpCode, enableData)
    {
    }
}
