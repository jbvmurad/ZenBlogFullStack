using ZenBlog.Application.Services.ZenBlogService;
using ZenBlog.Domain.DTOs.SystemDTOs;

namespace ZenBlog.Application.Features.ZenBlogFeatures.CommentFeatures.Commands.UpdateComment;

public sealed class UpdateCommentCommandHandler
{
    private readonly ICommentService _commentService;

    public UpdateCommentCommandHandler(ICommentService commentService)
    {
        _commentService = commentService;
    }

    public async Task<MessageResponse> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
    {
        await _commentService.UpdateAsync(request, cancellationToken);
        return new MessageResponse("Comment updated successfully");
    }
}
