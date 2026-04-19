using Microsoft.AspNetCore.Http;

namespace ZenBlog.Application.Features.ZenBlogFeatures.SocialFeatures.Commands.UpdateSocial;

public sealed record UpdateSocialCommand(
    string Id,
    string? Title,
    string? Url,
    IFormFile? Icon);
