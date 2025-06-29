namespace HiveSpace.Domain.Exceptions;

public class DomainException : Exception
{
    public int HttpCode { get; } = 422;
    public Enum ErrorCode { get; }

    public string? Key { get; }
    public object? Value { get; }

    public DomainException(Enum errorCode, string? key = null, object? value = null)
        : base()
    {
        ErrorCode = errorCode;
        Key = key;
        Value = value;
    }
}
