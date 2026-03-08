namespace ZenBlog.Application.Features.UserFeatures.AuthFeatures.Commands.Login;

public sealed record LoginCommand(
    string Email,
    string Password);
