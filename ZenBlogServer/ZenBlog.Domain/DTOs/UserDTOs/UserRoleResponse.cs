namespace ZenBlog.Domain.DTOs.UserDTOs;

public sealed record UserRoleResponse(
    string Id,
    string UserId,
    string RoleId,
    string? UserFullName,
    string? UserEmail,
    string? RoleName,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
