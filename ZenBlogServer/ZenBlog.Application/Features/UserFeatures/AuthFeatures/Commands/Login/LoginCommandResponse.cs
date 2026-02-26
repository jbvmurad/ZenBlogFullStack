namespace ZenBlog.Application.Features.UserAttributeFeatures.AuthFeatures.Commands.Login;

public sealed record LoginCommandResponse(
    string Token,
    string RefreshToken,
    DateTime? RefreshTokenExpires,
    string UserId);
