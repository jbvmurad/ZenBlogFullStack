using ZenBlog.Application.Services.ZenBlogService;
using ZenBlog.Domain.DTOs.SystemDTOs;

namespace ZenBlog.Application.Features.ZenBlogFeatures.CommentFeatures.Commands.CreateComment;

public sealed class CreateCommentCommandHandler
{
    private readonly ICommentService _commentService;

    public CreateCommentCommandHandler(ICommentService commentService)
    {
        _commentService = commentService;
    }

    public async Task<MessageResponse> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        await _commentService.CreateAsync(request, cancellationToken);
        return new MessageResponse("Comment created successfully.");
    }
}
