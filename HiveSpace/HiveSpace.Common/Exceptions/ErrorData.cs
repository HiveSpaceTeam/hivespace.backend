namespace HiveSpace.Common.Exceptions;
public class ErrorData(string key, string value)
{
    public string Key { get; set; } = key;
    public string Value { get; set; } = value;
}
