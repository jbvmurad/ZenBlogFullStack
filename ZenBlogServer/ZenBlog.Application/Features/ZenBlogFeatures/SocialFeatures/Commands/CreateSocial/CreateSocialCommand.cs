using Microsoft.AspNetCore.Http;

namespace ZenBlog.Application.Features.ZenBlogFeatures.SocialFeatures.Commands.CreateSocial;

public sealed record CreateSocialCommand(
    string Title,
    string Url,
    IFormFile Icon);
