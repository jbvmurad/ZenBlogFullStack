using ZenBlog.Application.Services.ZenBlogService;
using ZenBlog.Domain.DTOs.SystemDTOs;

namespace ZenBlog.Application.Features.ZenBlogFeatures.MessageFeatures.Commands.UpdateMessage;

public sealed class UpdateMessageCommandHandler
{
    private readonly IMessageService _messageService;

    public UpdateMessageCommandHandler(IMessageService messageService)
    {
        _messageService = messageService;
    }

    public async Task<MessageResponse> Handle(UpdateMessageCommand request, CancellationToken cancellationToken)
    {
        await _messageService.UpdateReadStateAsync(request, cancellationToken);
        return new MessageResponse("Message read state updated successfully.");
    }
}
