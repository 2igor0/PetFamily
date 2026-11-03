using System.Text.Json;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Caching;
using StackExchange.Redis;

namespace PetFamily.Infrastructure.Caching;

public sealed class RedisCacheService(
    IConnectionMultiplexer connection,
    ILogger<RedisCacheService> logger) : ICacheService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly IDatabase _database = connection.GetDatabase();

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var value = await _database.StringGetAsync(key);

            if (value.IsNullOrEmpty)
                return default;

            return JsonSerializer.Deserialize<T>(value.ToString(), JsonOptions);
        }
        catch (Exception exception) when (exception is RedisException or JsonException)
        {
            logger.LogWarning(exception, "Failed to read cache key {CacheKey}", key);

            return default;
        }
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan ttl,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var json = JsonSerializer.Serialize(value, JsonOptions);

            await _database.StringSetAsync(key, json, ttl);
        }
        catch (RedisException exception)
        {
            logger.LogWarning(exception, "Failed to write cache key {CacheKey}", key);
        }
    }

    public async Task<T> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan ttl,
        CancellationToken cancellationToken = default)
    {
        var cachedValue = await GetAsync<T>(key, cancellationToken);

        if (cachedValue is not null)
            return cachedValue;

        var value = await factory(cancellationToken);

        if (value is not null)
            await SetAsync(key, value, ttl, cancellationToken);

        return value;
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _database.KeyDeleteAsync(key);
        }
        catch (RedisException exception)
        {
            logger.LogWarning(exception, "Failed to remove cache key {CacheKey}", key);
        }
    }

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        try
        {
            foreach (var endpoint in connection.GetEndPoints())
            {
                var server = connection.GetServer(endpoint);

                if (!server.IsConnected)
                    continue;

                var keys = server
                    .Keys(pattern: $"{prefix}*", pageSize: 250)
                    .ToArray();

                if (keys.Length > 0)
                    await _database.KeyDeleteAsync(keys);
            }
        }
        catch (RedisException exception)
        {
            logger.LogWarning(exception, "Failed to remove cache keys with prefix {CachePrefix}", prefix);
        }
    }
}
