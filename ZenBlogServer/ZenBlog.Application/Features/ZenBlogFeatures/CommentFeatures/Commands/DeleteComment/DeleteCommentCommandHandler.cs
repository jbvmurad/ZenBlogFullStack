using ZenBlog.Application.Services.ZenBlogService;
using ZenBlog.Domain.DTOs.SystemDTOs;

namespace ZenBlog.Application.Features.ZenBlogFeatures.CommentFeatures.Commands.DeleteComment;

public sealed class DeleteCommentCommandHandler
{
    private readonly ICommentService _commentService;

    public DeleteCommentCommandHandler(ICommentService commentService)
    {
        _commentService = commentService;
    }

    public async Task<MessageResponse> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        await _commentService.DeleteAsync(request, cancellationToken);
        return new MessageResponse("Comment deleted successfully");
    }
}
