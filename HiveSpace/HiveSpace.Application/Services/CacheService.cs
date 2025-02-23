using HiveSpace.Application.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace HiveSpace.Application.Services;

public class CacheService : ICacheService
{
    private readonly StackExchange.Redis.IDatabase _cache;
    private readonly string _instanceName;

    public CacheService(IConfiguration config)
    {
        var redisConfig = config.GetSection("Redis:ConnectionString").Value;
        _instanceName = config.GetSection("Redis:InstanceName").Value ?? "";

        var options = ConfigurationOptions.Parse(redisConfig);
        options.AbortOnConnectFail = false;

        var redis = ConnectionMultiplexer.Connect(options);
        _cache = redis.GetDatabase();
    }

    public async Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var jsonData = JsonSerializer.Serialize(value);
        return await _cache.StringSetAsync(_instanceName + key, jsonData, expiry);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await _cache.StringGetAsync(_instanceName + key);
        return value.IsNullOrEmpty ? default : JsonSerializer.Deserialize<T>(value);
    }

    public async Task<bool> RemoveAsync(string key)
    {
        return await _cache.KeyDeleteAsync(_instanceName + key);
    }

    public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> getData, TimeSpan? expiry = null)
    {
        var value = await GetAsync<T>(key);

        if (value != null)
        {
            return value;
        }

        var data = await getData(); // Gọi hàm lấy dữ liệu từ DB/API
        await SetAsync(key, data, expiry); // Lưu vào Redis
        return data;
    }
}