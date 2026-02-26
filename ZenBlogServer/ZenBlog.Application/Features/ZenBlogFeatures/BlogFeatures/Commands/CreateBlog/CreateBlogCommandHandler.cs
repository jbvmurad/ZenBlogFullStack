using ZenBlog.Application.Services.ZenBlogService;
using Wolverine;
using ZenBlog.Domain.DTOs.SystemDTOs;
using ZenBlog.Domain.Events;

namespace ZenBlog.Application.Features.ZenBlogFeatures.BlogFeatures.Commands.CreateBlog;

public sealed class CreateBlogCommandHandler
{
    private readonly IBlogService _blogService;
    private readonly IMessageBus _bus;

    public CreateBlogCommandHandler(IBlogService blogService, IMessageBus bus)
    {
        _blogService = blogService;
        _bus = bus;
    }

    public async Task<MessageResponse> Handle(CreateBlogCommand request, CancellationToken cancellationToken)
    {
        var id = await _blogService.CreateAsync(request, cancellationToken);

        await _bus.PublishAsync(new BlogCreatedIntegrationEvent(
            BlogId: id,
            Title: request.Title!,
            CategoryId: request.CategoryId!,
            UserId: request.UserId!,
            CreatedAtUtc: DateTime.UtcNow));

        return new MessageResponse ( "Blog created successfully.");

    }
}
