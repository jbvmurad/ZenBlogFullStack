namespace ZenBlog.Application.Features.ZenBlogFeatures.BlogFeatures.Commands.UpdateBlog;

public sealed record UpdateBlogCommand(
    string Id,
    string Title,
    string CoverImage,
    string BlogImage,
    string Description,
    string CategoryId,
    string UserId);
