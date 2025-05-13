namespace HiveSpace.Common.Exceptions;
public class BadRequestException : BaseException
{
    private static readonly int _httpCode = 400;

    public BadRequestException(List<ErrorCode> errorCodeList, bool? enableData = false) 
        : base(errorCodeList, _httpCode, enableData)
    {
    }

    public BadRequestException(List<ErrorCode> errorCodeList, Exception inner, bool? enableData =false) : base(errorCodeList, inner, _httpCode, enableData)
    {
    }
}
