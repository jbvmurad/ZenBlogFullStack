using Microsoft.AspNetCore.Http;

namespace ZenBlog.Application.Features.ZenBlogFeatures.BlogFeatures.Commands.UpdateBlog;

public sealed record UpdateBlogCommand(
    string Id,
    string? Title,
    string? Description,
    string? CategoryId,
    string? UserId,
    IFormFile? CoverImage,
    IFormFile? BlogImage);
