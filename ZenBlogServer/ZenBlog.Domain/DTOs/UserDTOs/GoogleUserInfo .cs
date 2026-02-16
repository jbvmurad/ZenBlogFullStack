namespace ZenBlog.Domain.DTOs.UserDTOs;

public sealed record GoogleUserInfo(
    string Email,
    string FullName,
    string Subject);
