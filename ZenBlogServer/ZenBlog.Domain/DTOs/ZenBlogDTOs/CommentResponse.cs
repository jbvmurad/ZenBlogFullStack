namespace ZenBlog.Domain.DTOs.ZenBlogResponses;

public sealed record CommentResponse(
    string Id,
    string FirstName,
    string LastName,
    string Email,
    string Body,
    string BlogId,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string? CommenterImageUrl = null);
