namespace ZenBlog.Application.Features.UserFeatures.AuthFeatures.Commands.Login;

public sealed record LoginCommandResponse(
    string Token,
    string RefreshToken,
    DateTime? RefreshTokenExpires,
    string UserId);
