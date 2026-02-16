namespace ZenBlog.Domain.DTOs.UserDTOs;

public sealed record UserParameters(
    string FullName,
    string Email,
    string UserName);
