using Microsoft.AspNetCore.Http;

namespace ZenBlog.Application.Features.ZenBlogFeatures.SocialFeatures.Commands.CreateSocialWithMedia;

public sealed record CreateSocialWithMediaCommand(
    IFormFile? Icon,
    string? Title,
    string? Url);
