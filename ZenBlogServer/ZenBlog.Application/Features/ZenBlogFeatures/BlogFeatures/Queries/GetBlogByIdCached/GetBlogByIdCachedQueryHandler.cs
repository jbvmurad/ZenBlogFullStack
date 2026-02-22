using ZenBlog.Application.Services.Caching;
using ZenBlog.Application.Services.ZenBlogService;
using ZenBlog.Domain.Entities.ZenBlogEntities;

namespace ZenBlog.Application.Features.ZenBlogFeatures.BlogFeatures.Queries.GetBlogByIdCached;

public sealed class GetBlogByIdCachedQueryHandler
{
    private readonly IBlogService _blogService;
    private readonly ICacheService _cache;

    public GetBlogByIdCachedQueryHandler(IBlogService blogService, ICacheService cache)
    {
        _blogService = blogService;
        _cache = cache;
    }

    public async Task<Blog?> Handle(GetBlogByIdCachedQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Id))
            return null;

        var key = CacheKeys.BlogById(request.Id);
        var cached = await _cache.GetAsync<Blog>(key, cancellationToken);
        if (cached is not null)
            return cached;

        var blog = await _blogService.GetByIdAsync(request.Id, cancellationToken);
        if (blog is not null)
        {
            await _cache.SetAsync(key, blog, TimeSpan.FromMinutes(2), cancellationToken);
        }

        return blog;
    }
}
