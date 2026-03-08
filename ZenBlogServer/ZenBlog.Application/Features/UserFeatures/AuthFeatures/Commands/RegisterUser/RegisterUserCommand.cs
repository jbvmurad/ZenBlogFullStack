namespace ZenBlog.Application.Features.UserFeatures.AuthFeatures.Commands.RegisterUser;

public sealed record RegisterUserCommand(
    string FullName,
    string Email,
    string Password,
    string ConfirmPassword);
