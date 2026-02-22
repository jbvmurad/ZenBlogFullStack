using GenericRepository;
using ZenBlog.Domain.Entities.ZenBlogEntities;
using ZenBlog.Domain.Repositories.ZenBlogRepositories;
using ZenBlog.Persistance.Context;

namespace ZenBlog.Persistance.Repositories.ZenBlogRepositories;

public sealed class SubCommentRepository : Repository<SubComment, ZenBlogContext>, ISubCommentRepository
{
    public SubCommentRepository(ZenBlogContext context) : base(context) { }
}
