namespace ZenBlog.Domain.Events;

public sealed record EmailConfirmationRequestedIntegrationEvent(
    string UserId,
    string Email,
    string Token,
    DateTime RequestedAtUtc);
