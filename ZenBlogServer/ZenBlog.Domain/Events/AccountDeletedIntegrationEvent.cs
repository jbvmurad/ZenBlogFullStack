namespace ZenBlog.Domain.Events;

public sealed record AccountDeletedIntegrationEvent(
    string UserId,
    string Email,
    DateTime DeletedAtUtc);
