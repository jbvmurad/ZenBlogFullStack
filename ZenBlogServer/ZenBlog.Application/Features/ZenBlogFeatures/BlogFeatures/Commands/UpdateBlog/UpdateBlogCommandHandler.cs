using ZenBlog.Application.Services.ZenBlogService;
using ZenBlog.Domain.DTOs.SystemDTOs;

namespace ZenBlog.Application.Features.ZenBlogFeatures.BlogFeatures.Commands.UpdateBlog;

public sealed class UpdateBlogCommandHandler
{
    private readonly IBlogService _blogService;

    public UpdateBlogCommandHandler(IBlogService blogService)
    {
        _blogService = blogService;
    }

    public async Task<MessageResponse> Handle(UpdateBlogCommand request, CancellationToken cancellationToken)
    {
        await _blogService.UpdateAsync(request, cancellationToken);
        return new MessageResponse("Blog updated successfully");
    }
}
