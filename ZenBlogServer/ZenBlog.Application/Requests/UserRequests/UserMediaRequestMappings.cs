using ZenBlog.Application.Features.UserFeatures.AuthFeatures.Commands.UpdateUserWithMedia;

namespace ZenBlog.Application.Requests.UserRequests;

public static class UserMediaRequestMappings
{
    public static UpdateUserWithMediaCommand ToUpdateUserWithMediaCommand(
        this UpdateUserMediaOptionalRequest media,
        string id,
        string? fullName,
        string? email,
        string? phoneNumber,
        string? password)
        => new(
            id,
            media.Image,
            fullName,
            email,
            phoneNumber,
            password);
}
