namespace ZenBlog.Application.Features.ZenBlogFeatures.MessageFeatures.Commands.CreateMessage;

public sealed record CreateMessageCommand(
    string Name,
    string Email,
    string Subject,
    string MessageBody);
