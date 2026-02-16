using ZenBlog.Application.Services.ZenBlogService;
using ZenBlog.Domain.DTOs.SystemDTOs;

namespace ZenBlog.Application.Features.ZenBlogFeatures.SubCommentFeatures.Commands.DeleteSubComment;

public sealed class DeleteSubCommentCommandHandler
{
    private readonly ISubCommentService _subCommentService;

    public DeleteSubCommentCommandHandler(ISubCommentService subCommentService)
    {
        _subCommentService = subCommentService;
    }

    public async Task<MessageResponse> Handle(DeleteSubCommentCommand request, CancellationToken cancellationToken)
    {
        await _subCommentService.DeleteAsync(request, cancellationToken);
        return new MessageResponse("SubComment deleted successfully");
    }
}
