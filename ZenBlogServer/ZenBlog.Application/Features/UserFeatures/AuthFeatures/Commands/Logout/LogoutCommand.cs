namespace ZenBlog.Application.Features.UserFeatures.AuthFeatures.Commands.Logout;

public sealed record LogoutCommand(string UserId, string RefreshToken);
