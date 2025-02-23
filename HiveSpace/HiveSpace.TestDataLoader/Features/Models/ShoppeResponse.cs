namespace HiveSpace.TestDataLoader.Features.Models;

public class ShoppeResponse<T>
{
    public int Code { get; set; }
    public string Message { get; set; } = string.Empty;
    public string UserMessage { get; set; } = string.Empty;
    public DataContainer<T> Data { get; set; } = new DataContainer<T>();
}
public class DataContainer<T>
{
    public List<T> List { get; set; } = [];
}