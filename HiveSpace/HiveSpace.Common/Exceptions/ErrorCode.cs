namespace HiveSpace.Common.Exceptions;
public class ErrorCode
{
    public Enum? Code { get; set; }
    public List<ErrorData>? Data { get; set; }
    public string? Source { get; set; }
}
