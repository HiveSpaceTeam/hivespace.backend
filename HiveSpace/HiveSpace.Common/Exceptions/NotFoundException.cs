using HiveSpace.Common.Exceptions.Models;

namespace HiveSpace.Common.Exceptions;

public class NotFoundException : BaseException
{
    private static readonly int _httpCode = 404;

    public NotFoundException(List<ErrorCode> errorCodeList, bool? enableData = false) 
        : base(errorCodeList, _httpCode, enableData)
    {
    }

    public NotFoundException(List<ErrorCode> errorCodeList, Exception inner, bool? enableData = false) 
        : base(errorCodeList, inner, _httpCode, enableData)
    {
    }
}
