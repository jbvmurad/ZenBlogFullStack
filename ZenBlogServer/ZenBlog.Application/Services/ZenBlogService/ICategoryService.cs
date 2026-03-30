using ZenBlog.Application.Features.ZenBlogFeatures.CategoryFeatures.Commands.CreateCategory;
using ZenBlog.Application.Features.ZenBlogFeatures.CategoryFeatures.Commands.DeleteCategory;
using ZenBlog.Application.Features.ZenBlogFeatures.CategoryFeatures.Commands.UpdateCategory;
using ZenBlog.Domain.DTOs.ZenBlogResponses;
using ZenBlog.Domain.Entities.ZenBlogEntities;

namespace ZenBlog.Application.Services.ZenBlogService;

public interface ICategoryService
{
    IQueryable<CategoryResponse> GetAllCategories();
    Task CreateAsync(CreateCategoryCommand request, CancellationToken cancellationToken);
    Task UpdateAsync(UpdateCategoryCommand request, CancellationToken cancellationToken);
    Task DeleteAsync(DeleteCategoryCommand request, CancellationToken cancellationToken);
}
