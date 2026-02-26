using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Wolverine;
using ZenBlog.Application.Features.ZenBlogFeatures.SubCommentFeatures.Commands.CreateSubComment;
using ZenBlog.Application.Features.ZenBlogFeatures.SubCommentFeatures.Commands.DeleteSubComment;
using ZenBlog.Application.Features.ZenBlogFeatures.SubCommentFeatures.Commands.UpdateSubComment;
using ZenBlog.Application.Services.ZenBlogService;
using ZenBlog.Domain.DTOs.SystemDTOs;
using ZenBlog.Domain.Entities.ZenBlogEntities;
using ZenBlog.Presentation.Controllers.Abstraction;

namespace ZenBlog.Presentation.Controllers.ZenBlogControllers;

[ApiController]
[Route("api/[controller]")]
public sealed class SubCommentController :APIController
{
    private readonly ISubCommentService _subCommentService;
    public SubCommentController(ISubCommentService subCommentService, IMessageBus bus) : base(bus)
    {
        _subCommentService= subCommentService;
    }

    [HttpGet]
    [EnableQuery]
    public IQueryable<SubComment> GetAll() => _subCommentService.GetAllSubComments();

    [HttpPost]
    public async Task<IActionResult> Create(CreateSubCommentCommand request, CancellationToken cancellationToken)
    {
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request, cancellationToken);
        return Ok(response);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        DeleteSubCommentCommand request = new(id.ToString());
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request, cancellationToken);
        return Ok(response);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateSubCommentCommand request, CancellationToken cancellationToken)
    {
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request);
        return Ok(response);
    }
}
