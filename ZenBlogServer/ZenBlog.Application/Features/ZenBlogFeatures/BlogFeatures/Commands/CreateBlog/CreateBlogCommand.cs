using Microsoft.AspNetCore.Http;

namespace ZenBlog.Application.Features.ZenBlogFeatures.BlogFeatures.Commands.CreateBlog;

public sealed record CreateBlogCommand(
    string Title,
    string Description,
    string CategoryId,
    string UserId,
    IFormFile CoverImage,
    IFormFile BlogImage);
