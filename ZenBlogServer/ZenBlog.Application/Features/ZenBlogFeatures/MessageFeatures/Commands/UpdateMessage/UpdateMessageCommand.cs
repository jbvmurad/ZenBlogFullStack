namespace ZenBlog.Application.Features.ZenBlogFeatures.MessageFeatures.Commands.UpdateMessage;

public sealed record UpdateMessageCommand(
    string Id,
    bool IsRead);
