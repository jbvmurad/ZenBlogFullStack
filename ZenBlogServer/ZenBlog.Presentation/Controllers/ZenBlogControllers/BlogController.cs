using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Wolverine;
using ZenBlog.Application.Features.ZenBlogFeatures.BlogFeatures.Commands.CreateBlog;
using ZenBlog.Application.Features.ZenBlogFeatures.BlogFeatures.Commands.DeleteBlog;
using ZenBlog.Application.Features.ZenBlogFeatures.BlogFeatures.Commands.UpdateBlog;
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
    public BlogController(IBlogService blogService,IMessageBus bus) : base(bus)
    {
        _blogService=blogService;
    }

    [HttpGet]
    [EnableQuery]
    public IQueryable<Blog>GetAll() => _blogService.GetAllBlogs();

    [HttpPost]
    public async Task<IActionResult> Create(CreateBlogCommand request, CancellationToken cancellationToken)
    {
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request, cancellationToken);
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
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request);
        return Ok(response);
    }
}
