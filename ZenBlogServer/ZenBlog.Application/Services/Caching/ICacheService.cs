namespace ZenBlog.Application.Services.Caching;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken);
    Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken cancellationToken);
    Task RemoveAsync(string key, CancellationToken cancellationToken);
}

public static class CacheKeys
{
    public static string BlogById(string id) => $"blog:{id}";
}
