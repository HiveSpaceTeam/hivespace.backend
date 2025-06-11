using HiveSpace.Application.Helpers;
using HiveSpace.Application.Interfaces;
using HiveSpace.Application.Models.AppSettings;
using HiveSpace.Domain.Enums;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Text.Json;

namespace HiveSpace.Application.Services;

public class CacheService : ICacheService
{
    private readonly string _instanceName;
    private readonly IMemoryCache _memoryCache;
    private readonly IDatabase _redisDb;
    private readonly TimeSpan _defaultExpiration;

    public CacheService(IMemoryCache memoryCache, IDatabase redisDb, IOptions<RedisOption> redisOption)
    {
        _memoryCache = memoryCache;
        _redisDb = redisDb;
        var options = redisOption.Value;
        _instanceName = options.InstanceName;
        _defaultExpiration = options.DefaultExpiration;
    }

    private string GetPrefixedKey(string key) => $"{_instanceName}:{key}";

    public async Task<T?> GetAsync<T>(string key)
    {
        var prefixedKey = GetPrefixedKey(key);

        if (_memoryCache.TryGetValue(prefixedKey, out T? value))
            return value;

        var redisValue = await _redisDb.StringGetAsync(prefixedKey).ConfigureAwait(false);
        if (!redisValue.IsNullOrEmpty)
        {
            value = JsonSerializer.Deserialize<T>(redisValue!);
            if (value is not null)
                _memoryCache.Set(prefixedKey, value, _defaultExpiration);
            return value;
        }

        return default;
    }

    public async Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        var prefixedKey = GetPrefixedKey(key);
        var expiry = expiration ?? _defaultExpiration;
        _memoryCache.Set(prefixedKey, value, expiry);
        var serialized = JsonSerializer.Serialize(value);
        return await _redisDb.StringSetAsync(prefixedKey, serialized, expiry, When.Always, CommandFlags.None).ConfigureAwait(false);
    }

    public async Task<bool> RemoveAsync(string key)
    {
        var prefixedKey = GetPrefixedKey(key);
        _memoryCache.Remove(prefixedKey);
        return await _redisDb.KeyDeleteAsync(prefixedKey).ConfigureAwait(false);
    }

    public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> getData, TimeSpan? expiry = null)
    {
        var value = await GetAsync<T>(key).ConfigureAwait(false);
        if (value is not null)
            return value;

        var data = await getData().ConfigureAwait(false);
        await SetAsync(key, data, expiry).ConfigureAwait(false);
        return data;
    }
}
