using ZenBlog.Application.Services.ZenBlogService;
using ZenBlog.Domain.DTOs.SystemDTOs;

namespace ZenBlog.Application.Features.ZenBlogFeatures.CategoryFeatures.Commands.CreateCategory;

public sealed class CreateCategoryCommandHandler
{
    private readonly ICategoryService _categoryService;

    public CreateCategoryCommandHandler(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<MessageResponse> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        await _categoryService.CreateAsync(request, cancellationToken);
        return new MessageResponse("Category created successfully.");
    }
}
