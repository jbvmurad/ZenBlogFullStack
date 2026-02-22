namespace ZenBlog.Application.Features.UserAttributeFeatures.AuthFeatures.Commands.ConfirmEmail;

public sealed record ConfirmEmailCommand(
    string UserId,
    string Token);
