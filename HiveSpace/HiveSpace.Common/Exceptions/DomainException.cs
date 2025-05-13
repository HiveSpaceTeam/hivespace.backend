namespace HiveSpace.Common.Exceptions;

public class DomainException : BaseException
{
    private static readonly int _httpCode = 422;

    public DomainException(List<ErrorCode> errorCodeList, bool? enableData = false)
        : base(errorCodeList, _httpCode, enableData)
    {
    }

    public DomainException(List<ErrorCode> errorCodeList, Exception inner, bool? enableData = false)
        : base(errorCodeList, inner, _httpCode, enableData)
    {
    }
}
