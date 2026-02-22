using Microsoft.AspNetCore.Http;

namespace ZenBlog.Application.Features.ZenBlogFeatures.BlogFeatures.Commands.UpdateBlogWithMedia;

public sealed record UpdateBlogWithMediaCommand(
    string Id,
    IFormFile? CoverImage,
    IFormFile? BlogImage,
    string? Title,
    string? Description,
    string? CategoryId,
    string? UserId);
