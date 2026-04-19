namespace ZenBlog.Domain.DTOs.ZenBlogResponses;

public sealed record BlogResponse(
    string Id,
    string Title,
    string CoverImage,
    string BlogImage,
    string Description,
    string CategoryId,
    string? CategoryName,
    string UserId,
    string? UserFullName,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
