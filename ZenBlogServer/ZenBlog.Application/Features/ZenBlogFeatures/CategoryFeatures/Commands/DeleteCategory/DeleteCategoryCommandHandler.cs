using ZenBlog.Application.Services.ZenBlogService;
using ZenBlog.Domain.DTOs.SystemDTOs;

namespace ZenBlog.Application.Features.ZenBlogFeatures.CategoryFeatures.Commands.DeleteCategory;

public sealed class DeleteCategoryCommandHandler
{
    private readonly ICategoryService _categoryService;

    public DeleteCategoryCommandHandler(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<MessageResponse> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        await _categoryService.DeleteAsync(request, cancellationToken);
        return new MessageResponse("Category deleted successfully");
    }
}
