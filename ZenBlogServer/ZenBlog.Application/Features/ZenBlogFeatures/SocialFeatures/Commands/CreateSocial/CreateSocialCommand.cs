namespace ZenBlog.Application.Features.ZenBlogFeatures.SocialFeatures.Commands.CreateSocial;

public sealed record CreateSocialCommand(
    string Title,
    string Url,
    string Icon);
