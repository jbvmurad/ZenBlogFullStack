using ZenBlog.Application.Services.ZenBlogService;
using MassTransit;
using ZenBlog.Domain.DTOs.SystemDTOs;
using ZenBlog.Domain.Events;

namespace ZenBlog.Application.Features.ZenBlogFeatures.BlogFeatures.Commands.CreateBlog;

public sealed class CreateBlogCommandHandler
{
    private readonly IBlogService _blogService;
    private readonly IPublishEndpoint _publish;

    public CreateBlogCommandHandler(IBlogService blogService, IPublishEndpoint publish)
    {
        _blogService = blogService;
        _publish = publish;
    }

    public async Task<MessageResponse> Handle(CreateBlogCommand request, CancellationToken cancellationToken)
    {
        var id = await _blogService.CreateAsync(request, cancellationToken);

        await _publish.Publish(
            new BlogCreatedIntegrationEvent(
                BlogId: id,
                Title: request.Title!,
                CategoryId: request.CategoryId!,
                UserId: request.UserId!,
                CreatedAtUtc: DateTime.UtcNow),
            cancellationToken);

        return new MessageResponse ( "Blog created successfully.");

    }
}
