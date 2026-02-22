using ZenBlog.Application.Features.ZenBlogFeatures.BlogFeatures.Commands.CreateBlogWithMedia;
using ZenBlog.Application.Features.ZenBlogFeatures.BlogFeatures.Commands.UpdateBlogWithMedia;

namespace ZenBlog.Application.Requests.BlogRequests;

public static class BlogMediaRequestMappings
{
    public static CreateBlogWithMediaCommand ToCreateBlogWithMediaCommand(
        this CreateBlogMediaOnlyRequest media,
        string? title,
        string? description,
        string? categoryId,
        string? userId)
        => new(
            media.CoverImage,
            media.BlogImage,
            title,
            description,
            categoryId,
            userId);

    public static UpdateBlogWithMediaCommand ToUpdateBlogWithMediaCommand(
        this UpdateBlogMediaOptionalRequest media,
        string id,
        string? title,
        string? description,
        string? categoryId,
        string? userId)
        => new(
            id,
            media.CoverImage,
            media.BlogImage,
            title,
            description,
            categoryId,
            userId);
}
