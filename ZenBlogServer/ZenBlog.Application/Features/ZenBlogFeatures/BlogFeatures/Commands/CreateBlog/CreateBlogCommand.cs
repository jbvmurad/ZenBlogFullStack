namespace ZenBlog.Application.Features.ZenBlogFeatures.BlogFeatures.Commands.CreateBlog;

public sealed record CreateBlogCommand(
    string Title,
    string CoverImage,
    string BlogImage,
    string Description,
    string CategoryId,
    string UserId);
