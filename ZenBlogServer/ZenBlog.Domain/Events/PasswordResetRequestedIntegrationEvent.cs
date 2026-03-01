namespace ZenBlog.Domain.Events;

public sealed record PasswordResetRequestedIntegrationEvent(
    string UserId,
    string Email,
    string Token,
    DateTime RequestedAtUtc);
