namespace ZenBlog.Domain.Events;

public sealed record BlogCreatedIntegrationEvent(
    string BlogId,
    string Title,
    string CategoryId,
    string UserId,
    DateTime CreatedAtUtc);
