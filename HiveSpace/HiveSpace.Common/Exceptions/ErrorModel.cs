namespace HiveSpace.Common.Exceptions;
public class ErrorModel
{
    public List<ErrorCodeDto> Errors { get; set; } = [];
    public string Status { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string TraceId { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
}

