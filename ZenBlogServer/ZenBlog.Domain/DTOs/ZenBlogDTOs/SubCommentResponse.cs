namespace ZenBlog.Domain.DTOs.ZenBlogResponses;

public sealed record SubCommentResponse(
    string Id,
    string FirstName,
    string LastName,
    string Email,
    string Body,
    string CommentId,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
