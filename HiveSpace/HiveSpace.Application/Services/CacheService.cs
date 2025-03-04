using HiveSpace.Application.Interfaces;
using HiveSpace.Application.Models.AppSettings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Text.Json;

namespace HiveSpace.Application.Services;

public class CacheService(IMemoryCache memoryCache, IDatabase redisDb, IOptions<RedisOption> redisOption) : ICacheService
{
    private readonly string _instanceName = redisOption.Value.InstanceName;
    private readonly IMemoryCache _memoryCache = memoryCache;
    private readonly IDatabase _redisDb = redisDb;
    private readonly TimeSpan _defaultExpiration = redisOption.Value.DefaultExpiration;
    private string GetPrefixedKey(string key) => $"{_instanceName}:{key}";

    public async Task<T?> GetAsync<T>(string key)
    {
        // Check in-memory cache first
        if (_memoryCache.TryGetValue(GetPrefixedKey(key), out T? value))
        {
            return value;
        }

        // Check Redis cache
        var redisValue = await _redisDb.StringGetAsync(GetPrefixedKey(key));
        if (!redisValue.IsNullOrEmpty)
        {
            value = JsonSerializer.Deserialize<T>(redisValue!);
            _memoryCache.Set(GetPrefixedKey(key), value, _defaultExpiration); // Store in-memory for faster access
            return value;
        }

        return default;
    }

    public async Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        var expiry = expiration ?? _defaultExpiration;
        _memoryCache.Set(GetPrefixedKey(key), value, expiry);
        return await _redisDb.StringSetAsync(GetPrefixedKey(key), JsonSerializer.Serialize(value), expiry);
    }

    public async Task<bool> RemoveAsync(string key)
    {
        _memoryCache.Remove(GetPrefixedKey(key));
        return await _redisDb.KeyDeleteAsync(GetPrefixedKey(key));
    }

    public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> getData, TimeSpan? expiry = null)
    {
        var value = await GetAsync<T>(key);

        if (value != null)
        {
            return value;
        }

        var data = await getData(); // Gọi hàm lấy dữ liệu từ DB/API
        var result = await SetAsync(key, data, expiry); // Lưu vào Redis
        return data;
    }
}