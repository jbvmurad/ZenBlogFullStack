using ZenBlog.Application.Services.ZenBlogService;
using ZenBlog.Domain.DTOs.SystemDTOs;

namespace ZenBlog.Application.Features.ZenBlogFeatures.SubCommentFeatures.Commands.UpdateSubComment;

public sealed class UpdateSubCommentCommandHandler
{
    private readonly ISubCommentService _subCommentService;

    public UpdateSubCommentCommandHandler(ISubCommentService subCommentService)
    {
        _subCommentService = subCommentService;
    }

    public async Task<MessageResponse> Handle(UpdateSubCommentCommand request, CancellationToken cancellationToken)
    {
        await _subCommentService.UpdateAsync(request, cancellationToken);
        return new MessageResponse("SubComment updated successfully");
    }
}
