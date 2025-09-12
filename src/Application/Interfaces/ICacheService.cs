namespace Application.Interfaces;

public interface ICacheService
{
    static readonly TimeSpan ShortCache = TimeSpan.FromHours(1);
    static readonly TimeSpan LongCache = TimeSpan.FromDays(24);
    Task<T?> TryGetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task SetAsync(string key, object value, TimeSpan expiresAt, CancellationToken cancellationToken = default);
}
