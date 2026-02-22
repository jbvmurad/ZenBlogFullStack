using Microsoft.AspNetCore.Http;

namespace ZenBlog.Application.Features.ZenBlogFeatures.BlogFeatures.Commands.CreateBlogWithMedia;

public sealed record CreateBlogWithMediaCommand(
    IFormFile? CoverImage,
    IFormFile? BlogImage,
    string? Title,
    string? Description,
    string? CategoryId,
    string? UserId);
