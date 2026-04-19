namespace ZenBlog.Domain.DTOs.ZenBlogResponses;

public sealed record CategoryResponse(
    string Id,
    string Name,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
