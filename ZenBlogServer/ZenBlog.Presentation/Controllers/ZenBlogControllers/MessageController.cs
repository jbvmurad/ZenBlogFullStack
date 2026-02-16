using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Wolverine;
using ZenBlog.Application.Features.ZenBlogFeatures.MessageFeatures.Commands.CreateMessage;
using ZenBlog.Application.Features.ZenBlogFeatures.MessageFeatures.Commands.DeleteMessage;
using ZenBlog.Application.Features.ZenBlogFeatures.MessageFeatures.Commands.UpdateMessage;
using ZenBlog.Application.Services.ZenBlogService;
using ZenBlog.Domain.DTOs.SystemDTOs;
using ZenBlog.Domain.Entities.ZenBlogEntities;
using ZenBlog.Presentation.Controllers.Abstraction;

namespace ZenBlog.Presentation.Controllers.ZenBlogControllers;

[ApiController]
[Route("api/[controller]")]
public sealed class MessageController :APIController
{
    private readonly IMessageService _messageService;
    public MessageController(IMessageService messageService, IMessageBus bus) : base(bus)
    {
        _messageService = messageService;
    }

    [HttpGet]
    [EnableQuery]
    public IQueryable<Message> GetAll() => _messageService.GetAllMessages();

    [HttpPost]
    public async Task<IActionResult> Create(CreateMessageCommand request, CancellationToken cancellationToken)
    {
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request, cancellationToken);
        return Ok(response);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        DeleteMessageCommand request = new(id.ToString());
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request, cancellationToken);
        return Ok(response);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateMessageCommand request, CancellationToken cancellationToken)
    {
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request);
        return Ok(response);
    }
}
