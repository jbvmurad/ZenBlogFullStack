namespace ZenBlog.Domain.DTOs.ZenBlogResponses;

public sealed record ContactInfoResponse(
    string Id,
    string Address,
    string Email,
    string Phone,
    string MapUrl,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
