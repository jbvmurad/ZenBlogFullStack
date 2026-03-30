namespace ZenBlog.Domain.DTOs.ZenBlogResponses;

public sealed record MessageItemResponse(
    string Id,
    string Name,
    string Email,
    string Subject,
    string MessageBody,
    bool IsRead,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
