namespace HiveSpace.Common.Exceptions;
public class ErrorCodeDto
{
    public string Code { get; set; } = string.Empty;
    public string MessageCode { get; set; } = string.Empty;
    public Dictionary<string, string> Data { get; set; } = [];
    public string? Source { get; set; }
}
