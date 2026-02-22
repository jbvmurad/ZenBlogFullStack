namespace ZenBlog.Application.Features.UserAttributeFeatures.AuthFeatures.Commands.UpdateUser;

public sealed record UpdateUserCommand(
    string Id,
    string? FullName,
    string? Email,
    string? PhoneNumber,
    string? Password);
