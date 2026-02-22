using MassTransit;
using Microsoft.Extensions.Logging;
using ZenBlog.Domain.Events;

namespace ZenBlog.API.Messaging.Consumers;

public sealed class BlogCreatedConsumer : IConsumer<BlogCreatedIntegrationEvent>
{
    private readonly ILogger<BlogCreatedConsumer> _logger;

    public BlogCreatedConsumer(ILogger<BlogCreatedConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<BlogCreatedIntegrationEvent> context)
    {
        var msg = context.Message;
        _logger.LogInformation(
            "RabbitMQ consumed BlogCreatedIntegrationEvent. BlogId={BlogId}, Title={Title}, CategoryId={CategoryId}, UserId={UserId}",
            msg.BlogId, msg.Title, msg.CategoryId, msg.UserId);

        return Task.CompletedTask;
    }
}
