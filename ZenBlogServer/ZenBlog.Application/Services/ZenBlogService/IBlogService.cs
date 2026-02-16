using ZenBlog.Application.Features.ZenBlogFeatures.BlogFeatures.Commands.CreateBlog;
using ZenBlog.Application.Features.ZenBlogFeatures.BlogFeatures.Commands.DeleteBlog;
using ZenBlog.Application.Features.ZenBlogFeatures.BlogFeatures.Commands.UpdateBlog;
using ZenBlog.Domain.Entities.ZenBlogEntities;

namespace ZenBlog.Application.Services.ZenBlogService;

public interface IBlogService
{
    IQueryable<Blog> GetAllBlogs();

    Task CreateAsync(CreateBlogCommand request, CancellationToken cancellationToken);
    Task UpdateAsync(UpdateBlogCommand request, CancellationToken cancellationToken);
    Task DeleteAsync(DeleteBlogCommand request, CancellationToken cancellationToken);
}
