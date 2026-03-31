using ZenBlog.Application.Features.ZenBlogFeatures.CategoryFeatures.Commands.CreateCategory;
using ZenBlog.Application.Features.ZenBlogFeatures.CategoryFeatures.Commands.DeleteCategory;
using ZenBlog.Application.Features.ZenBlogFeatures.CategoryFeatures.Commands.UpdateCategory;
using ZenBlog.Domain.Entities.ZenBlogEntities;

namespace ZenBlog.Application.Services.ZenBlogService;

public interface ICategoryService
{
    IQueryable<Category> GetAllCategories();
    Task CreateAsync(CreateCategoryCommand request, CancellationToken cancellationToken);
    Task UpdateAsync(UpdateCategoryCommand request, CancellationToken cancellationToken);
    Task DeleteAsync(DeleteCategoryCommand request, CancellationToken cancellationToken);
}
