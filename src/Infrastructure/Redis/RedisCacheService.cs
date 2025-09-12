using Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Infrastructure.Redis;

public class RedisCacheService(IDistributedCache cache) : ICacheService
{
    public async Task<T?> TryGetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var data = await cache.GetAsync(key, cancellationToken);

            return data is null ? default : JsonSerializer.Deserialize<T>(data);
        }
        catch (Exception)
        {
            return default;
        }
    }

    public async Task SetAsync(string key, object value, TimeSpan expiresAt, CancellationToken cancellationToken = default)
    {
        try
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiresAt
            };

            var data = JsonSerializer.SerializeToUtf8Bytes(value);

            await cache.SetAsync(key, data, options, cancellationToken);
        }
        catch (Exception)
        {
            // TODO: Log
        }
    }
}
