using Microsoft.AspNetCore.Http;

namespace ZenBlog.Application.Features.UserFeatures.AuthFeatures.Commands.UpdateUser;

public sealed record UpdateUserCommand(
    string Id,
#nullable enable
    string? FullName,
    string? Email,
    string? PhoneNumber,
    string? Password,
    IFormFile? Image,
    bool RemoveImage = false
#nullable disable
    );