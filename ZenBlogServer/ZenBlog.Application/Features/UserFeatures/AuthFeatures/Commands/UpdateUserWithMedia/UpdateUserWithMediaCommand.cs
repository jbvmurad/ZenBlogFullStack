using Microsoft.AspNetCore.Http;

namespace ZenBlog.Application.Features.UserFeatures.AuthFeatures.Commands.UpdateUserWithMedia;

public sealed record UpdateUserWithMediaCommand(
    string Id,
    IFormFile? Image,
    string? FullName,
    string? Email,
    string? PhoneNumber,
    string? Password);
