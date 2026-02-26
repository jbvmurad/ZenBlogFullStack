using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Wolverine;
using ZenBlog.Application.Features.ZenBlogFeatures.BlogFeatures.Commands.CreateBlog;
using ZenBlog.Application.Features.ZenBlogFeatures.BlogFeatures.Commands.DeleteBlog;
using ZenBlog.Application.Features.ZenBlogFeatures.BlogFeatures.Commands.UpdateBlog;
using ZenBlog.Application.Features.ZenBlogFeatures.BlogFeatures.Queries.GetBlogByIdCached;
using ZenBlog.Application.Requests.BlogRequests;
using ZenBlog.Application.Services.ZenBlogService;
using ZenBlog.Domain.DTOs.SystemDTOs;
using ZenBlog.Domain.Entities.ZenBlogEntities;
using ZenBlog.Presentation.Controllers.Abstraction;

namespace ZenBlog.Presentation.Controllers.ZenBlogControllers;

[ApiController]
[Route("api/[controller]")]
public sealed class BlogController : APIController
{
    private readonly IBlogService _blogService;
    public BlogController(IBlogService blogService, IMessageBus bus) : base(bus)
    {
        _blogService = blogService;
    }

    [HttpGet]
    [EnableQuery]
    public IQueryable<Blog> GetAll() => _blogService.GetAllBlogs();

    [HttpGet("by-id-cached")]
    public async Task<IActionResult> GetByIdCached([FromQuery] string id, CancellationToken cancellationToken)
    {
        var blog = await _bus.InvokeAsync<Blog?>(new GetBlogByIdCachedQuery(id), cancellationToken);
        return Ok(blog);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateBlogCommand request, CancellationToken cancellationToken)
    {
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("with-media")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CreateWithMedia(
        [FromForm] CreateBlogMediaOnlyRequest media,
        [FromQuery] string Title,
        [FromQuery] string Description,
        [FromQuery] string CategoryId,
        [FromQuery] string UserId,
        CancellationToken cancellationToken)
    {
        var command = media.ToCreateBlogWithMediaCommand(Title, Description, CategoryId, UserId);
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(command, cancellationToken);
        return Ok(response);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        DeleteBlogCommand request = new(id.ToString());
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request, cancellationToken);
        return Ok(response);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateBlogCommand request, CancellationToken cancellationToken)
    {
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request, cancellationToken);
        return Ok(response);
    }

    [HttpPut("with-media")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateWithMedia(
        [FromForm] UpdateBlogMediaOptionalRequest media,
        [FromQuery] string id,
        [FromQuery] string? Title,
        [FromQuery] string? Description,
        [FromQuery] string? CategoryId,
        [FromQuery] string? UserId,
        CancellationToken cancellationToken)
    {
        var command = media.ToUpdateBlogWithMediaCommand(id, Title, Description, CategoryId, UserId);
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(command, cancellationToken);
        return Ok(response);
    }
}
