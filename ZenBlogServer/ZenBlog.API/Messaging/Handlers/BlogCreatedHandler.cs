using ZenBlog.Domain.Events;

namespace ZenBlog.API.Messaging.Handlers;

public sealed class BlogCreatedHandler
{
    private readonly ILogger<BlogCreatedHandler> _logger;

    public BlogCreatedHandler(ILogger<BlogCreatedHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(BlogCreatedIntegrationEvent message)
    {
        _logger.LogInformation(
            "RabbitMQ consumed BlogCreatedIntegrationEvent. BlogId={BlogId}, Title={Title}, CategoryId={CategoryId}, UserId={UserId}",
            message.BlogId, message.Title, message.CategoryId, message.UserId);

        return Task.CompletedTask;
    }
}
