using ZenBlog.Application.Services.ZenBlogService;
using ZenBlog.Domain.DTOs.SystemDTOs;

namespace ZenBlog.Application.Features.ZenBlogFeatures.MessageFeatures.Commands.DeleteMessage;

public sealed class DeleteMessageCommandHandler
{
    private readonly IMessageService _messageService;

    public DeleteMessageCommandHandler(IMessageService messageService)
    {
        _messageService = messageService;
    }

    public async Task<MessageResponse> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
    {
        await _messageService.DeleteAsync(request, cancellationToken);
        return new MessageResponse("Message deleted successfully");
    }
}
