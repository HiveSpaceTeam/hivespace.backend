namespace HiveSpace.Application.Models.AppSettings;

public class RedisOption
{
    public string ConnectionString { get; set; } = "";
    public string InstanceName { get; set; } = "";
    public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromMinutes(5);
}
