namespace ZenBlog.Application.Features.UserFeatures.AuthFeatures.Commands.CreateNewTokenByRefreshToken;

public sealed record CreateNewTokenByRefreshTokenCommand(
    string UserId,
    string RefreshToken);
