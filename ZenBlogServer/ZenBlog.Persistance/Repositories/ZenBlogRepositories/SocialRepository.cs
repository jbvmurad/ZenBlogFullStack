using GenericRepository;
using ZenBlog.Domain.Entities.ZenBlogEntities;
using ZenBlog.Domain.Repositories.ZenBlogRepositories;
using ZenBlog.Persistance.Context;

namespace ZenBlog.Persistance.Repositories.ZenBlogRepositories;

public sealed class SocialRepository: Repository<Social, ZenBlogContext>, ISocialRepository
{
    public SocialRepository(ZenBlogContext context) : base(context) { }
}
