using ZenBlog.Application.Services.ZenBlogService;
using ZenBlog.Domain.DTOs.SystemDTOs;

namespace ZenBlog.Application.Features.ZenBlogFeatures.SubCommentFeatures.Commands.CreateSubComment;

public sealed class CreateSubCommentCommandHandler
{
    private readonly ISubCommentService _subCommentService;

    public CreateSubCommentCommandHandler(ISubCommentService subCommentService)
    {
        _subCommentService = subCommentService;
    }

    public async Task<MessageResponse> Handle(CreateSubCommentCommand request, CancellationToken cancellationToken)
    {
        await _subCommentService.CreateAsync(request, cancellationToken);
        return new MessageResponse("SubComment created successfully.");
    }
}
