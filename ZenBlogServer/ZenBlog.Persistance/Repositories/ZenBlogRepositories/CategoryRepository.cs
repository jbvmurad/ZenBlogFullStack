using GenericRepository;
using ZenBlog.Domain.Entities.ZenBlogEntities;
using ZenBlog.Domain.Repositories.ZenBlogRepositories;
using ZenBlog.Persistance.Context;

namespace ZenBlog.Persistance.Repositories.ZenBlogRepositories;

public sealed class CategoryRepository :Repository<Category, ZenBlogContext>, ICategoryRepository
{
    public CategoryRepository(ZenBlogContext context) : base(context) { }
}
