namespace ZenBlog.Domain.DTOs.UserDTOs;

public sealed record UserResponse(
    string Id,
    string FullName,
    string? Email,
    string? PhoneNumber,
    string? ImageUrl,
    bool EmailConfirmed);
