namespace ZenBlog.Domain.DTOs.UserDTOs;

public sealed record UserResponse(
    string Id,
    string FullName,
    string? UserName,
    string? Email,
    string? PhoneNumber,
    string? ImageUrl,
    bool EmailConfirmed)
{
    public bool IsProtectedDashboardAdmin { get; init; }
}
