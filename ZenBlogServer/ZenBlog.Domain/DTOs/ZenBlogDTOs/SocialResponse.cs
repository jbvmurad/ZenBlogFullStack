namespace ZenBlog.Domain.DTOs.ZenBlogResponses;

public sealed record SocialResponse(
    string Id,
    string Title,
    string Url,
    string Icon,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
