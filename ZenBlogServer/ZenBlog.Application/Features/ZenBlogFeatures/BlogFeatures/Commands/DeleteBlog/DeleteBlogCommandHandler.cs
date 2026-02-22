using ZenBlog.Application.Services.ZenBlogService;
using ZenBlog.Domain.DTOs.SystemDTOs;

namespace ZenBlog.Application.Features.ZenBlogFeatures.BlogFeatures.Commands.DeleteBlog;

public sealed class DeleteBlogCommandHandler
{
    private readonly IBlogService _blogService;

    public DeleteBlogCommandHandler(IBlogService blogService)
    {
        _blogService = blogService;
    }

    public async Task<MessageResponse> Handle(DeleteBlogCommand request, CancellationToken cancellationToken)
    {
        await _blogService.DeleteAsync(request, cancellationToken);
        return new MessageResponse("Blog deleted successfully");
    }
}
