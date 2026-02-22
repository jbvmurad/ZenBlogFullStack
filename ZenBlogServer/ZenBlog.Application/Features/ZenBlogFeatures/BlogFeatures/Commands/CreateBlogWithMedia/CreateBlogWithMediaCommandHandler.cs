using ZenBlog.Application.Features.ZenBlogFeatures.BlogFeatures.Commands.CreateBlog;
using ZenBlog.Application.Services.ZenBlogService;
using MassTransit;
using ZenBlog.Domain.DTOs.SystemDTOs;
using ZenBlog.Domain.Events;

namespace ZenBlog.Application.Features.ZenBlogFeatures.BlogFeatures.Commands.CreateBlogWithMedia;

public sealed class CreateBlogWithMediaCommandHandler
{
    private readonly IBlogService _blogService;
    private readonly IPublishEndpoint _publish;

    public CreateBlogWithMediaCommandHandler(IBlogService blogService, IPublishEndpoint publish)
    {
        _blogService = blogService;
        _publish = publish;
    }

    public async Task<MessageResponse> Handle(CreateBlogWithMediaCommand request, CancellationToken cancellationToken)
    {
        var coverUrl = await _blogService.SaveBlogImageAsync(request.CoverImage!, cancellationToken);
        var blogUrl = await _blogService.SaveBlogImageAsync(request.BlogImage!, cancellationToken);

        var command = new CreateBlogCommand(
            request.Title!,
            coverUrl,
            blogUrl,
            request.Description!,
            request.CategoryId!,
            request.UserId!);

        var id = await _blogService.CreateAsync(command, cancellationToken);

        await _publish.Publish(
            new BlogCreatedIntegrationEvent(
                BlogId: id,
                Title: request.Title!,
                CategoryId: request.CategoryId!,
                UserId: request.UserId!,
                CreatedAtUtc: DateTime.UtcNow),
            cancellationToken);

        return new MessageResponse("Blog created successfully.");
    }
}
