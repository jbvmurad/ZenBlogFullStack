using ZenBlog.Application.Features.ZenBlogFeatures.SocialFeatures.Commands.CreateSocialWithMedia;
using ZenBlog.Application.Features.ZenBlogFeatures.SocialFeatures.Commands.UpdateSocialWithMedia;

namespace ZenBlog.Application.Requests.SocialRequests;

public static class SocialMediaRequestMappings
{
    public static CreateSocialWithMediaCommand ToCreateSocialWithMediaCommand(
        this CreateSocialMediaOnlyRequest media,
        string? title,
        string? url)
        => new(
            media.Icon,
            title,
            url);

    public static UpdateSocialWithMediaCommand ToUpdateSocialWithMediaCommand(
        this UpdateSocialMediaOptionalRequest media,
        string id,
        string? title,
        string? url)
        => new(
            id,
            media.Icon,
            title,
            url);
}
