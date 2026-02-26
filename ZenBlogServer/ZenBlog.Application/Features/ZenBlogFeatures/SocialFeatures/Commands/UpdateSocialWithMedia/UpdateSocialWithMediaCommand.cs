using Microsoft.AspNetCore.Http;

namespace ZenBlog.Application.Features.ZenBlogFeatures.SocialFeatures.Commands.UpdateSocialWithMedia;

public sealed record UpdateSocialWithMediaCommand(
    string Id,
    IFormFile? Icon,
    string? Title,
    string? Url);
