using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Wolverine;
using ZenBlog.Application.Features.ZenBlogFeatures.CommentFeatures.Commands.CreateComment;
using ZenBlog.Application.Features.ZenBlogFeatures.CommentFeatures.Commands.DeleteComment;
using ZenBlog.Application.Features.ZenBlogFeatures.CommentFeatures.Commands.UpdateComment;
using ZenBlog.Application.Services.ZenBlogService;
using ZenBlog.Domain.DTOs.SystemDTOs;
using ZenBlog.Domain.Entities.ZenBlogEntities;
using ZenBlog.Presentation.Controllers.Abstraction;

namespace ZenBlog.Presentation.Controllers.ZenBlogControllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CommentController :APIController
{
    private readonly ICommentService _commentService;

    public CommentController(ICommentService commentService, IMessageBus bus) : base(bus)
    {
        _commentService = commentService;
    }

    [HttpGet]
    [EnableQuery]
    public IQueryable<Comment> GetAll() => _commentService.GetAllComments();

    [HttpPost]
    public async Task<IActionResult> Create(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request, cancellationToken);
        return Ok(response);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        DeleteCommentCommand request = new(id.ToString());
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request, cancellationToken);
        return Ok(response);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateCommentCommand request, CancellationToken cancellationToken)
    {
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request);
        return Ok(response);
    }
}
