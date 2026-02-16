using ZenBlog.Application.Services.ZenBlogService;
using ZenBlog.Domain.DTOs.SystemDTOs;

namespace ZenBlog.Application.Features.ZenBlogFeatures.BlogFeatures.Commands.CreateBlog;

public sealed class CreateBlogCommandHandler
{
    private readonly IBlogService _blogService;

    public CreateBlogCommandHandler(IBlogService blogService)
    {
        _blogService = blogService;
    }

    public async Task<MessageResponse> Handle(CreateBlogCommand request, CancellationToken cancellationToken)
    {
        await _blogService.CreateAsync(request, cancellationToken);
        return new MessageResponse ( "Blog created successfully.");

    }
}
