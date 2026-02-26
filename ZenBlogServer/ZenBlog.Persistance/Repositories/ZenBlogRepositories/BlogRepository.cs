using GenericRepository;
using ZenBlog.Domain.Entities.ZenBlogEntities;
using ZenBlog.Domain.Repositories.ZenBlogRepositories;
using ZenBlog.Persistance.Context;

namespace ZenBlog.Persistance.Repositories.ZenBlogRepositories;

public sealed class BlogRepository : Repository<Blog, ZenBlogContext>, IBlogRepository
{
    public BlogRepository(ZenBlogContext context) : base(context) { }
}
