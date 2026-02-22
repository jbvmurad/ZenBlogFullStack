using ZenBlog.Application.Services.ZenBlogService;
using ZenBlog.Domain.DTOs.SystemDTOs;

namespace ZenBlog.Application.Features.ZenBlogFeatures.MessageFeatures.Commands.CreateMessage;

public sealed class CreateMessageCommandHandler
{
    private readonly IMessageService _messageService;

    public CreateMessageCommandHandler(IMessageService messageService)
    {
        _messageService = messageService;
    }

    public async Task<MessageResponse> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
    {
        await _messageService.CreateAsync(request, cancellationToken);
        return new MessageResponse("Message created successfully.");
    }
}
