namespace ZenBlog.Application.Features.UserFeatures.AuthFeatures.Commands.ConfirmEmail;

public sealed record ConfirmEmailCommand(
    string UserId,
    string Token);
