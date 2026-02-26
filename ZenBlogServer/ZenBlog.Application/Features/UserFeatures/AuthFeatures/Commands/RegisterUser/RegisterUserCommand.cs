namespace ZenBlog.Application.Features.UserAttributeFeatures.AuthFeatures.Commands.RegisterUser;

public sealed record RegisterUserCommand(
    string FullName,
    string Email,
    string Password);
