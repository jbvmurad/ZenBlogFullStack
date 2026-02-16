using GenericRepository;
using ZenBlog.Domain.Entities.ZenBlogEntities;
using ZenBlog.Domain.Repositories.ZenBlogRepositories;
using ZenBlog.Persistance.Context;

namespace ZenBlog.Persistance.Repositories.ZenBlogRepositories;

public sealed class CommentRepository : Repository<Comment, ZenBlogContext>, ICommentRepository
{
    public CommentRepository(ZenBlogContext context) : base(context) { }

}
