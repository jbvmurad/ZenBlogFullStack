using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Wolverine;
using ZenBlog.Application.Features.ZenBlogFeatures.CategoryFeatures.Commands.CreateCategory;
using ZenBlog.Application.Features.ZenBlogFeatures.CategoryFeatures.Commands.DeleteCategory;
using ZenBlog.Application.Features.ZenBlogFeatures.CategoryFeatures.Commands.UpdateCategory;
using ZenBlog.Application.Services.ZenBlogService;
using ZenBlog.Domain.DTOs.SystemDTOs;
using ZenBlog.Domain.Entities.ZenBlogEntities;
using ZenBlog.Presentation.Controllers.Abstraction;

namespace ZenBlog.Presentation.Controllers.ZenBlogControllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CategoryController : APIController
{
    private readonly ICategoryService _categoryService;
    public CategoryController(ICategoryService categoryService,IMessageBus bus) : base(bus)
    {
        _categoryService=categoryService;
    }

    [HttpGet]
    [EnableQuery]
    public IQueryable<Category> GetAll() => _categoryService.GetAllCategories();

    [HttpPost]
    public async Task<IActionResult> Create(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request, cancellationToken);
        return Ok(response);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        DeleteCategoryCommand request = new(id.ToString());
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request, cancellationToken);
        return Ok(response);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request);
        return Ok(response);
    }
}
