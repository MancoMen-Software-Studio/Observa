using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace Observa.Infrastructure.Caching;

/// <summary>
/// Servicio de cache distribuido basado en Redis.
/// </summary>
public sealed class RedisCacheService
{
    private static readonly TimeSpan s_defaultExpiration = TimeSpan.FromMinutes(5);

    private readonly IDistributedCache _cache;

    public RedisCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        where T : class
    {
        var cached = await _cache.GetStringAsync(key, cancellationToken);

        if (cached is null)
        {
            return null;
        }

        return JsonSerializer.Deserialize<T>(cached);
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
        where T : class
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? s_defaultExpiration
        };

        var serialized = JsonSerializer.Serialize(value);

        await _cache.SetStringAsync(key, serialized, options, cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync(key, cancellationToken);
    }
}
