using GenericRepository;
using ZenBlog.Domain.Entities.ZenBlogEntities;
using ZenBlog.Domain.Repositories.ZenBlogRepositories;
using ZenBlog.Persistance.Context;

namespace ZenBlog.Persistance.Repositories.ZenBlogRepositories;

public sealed class MessageRepository: Repository<Message, ZenBlogContext>, IMessageRepository
{
    public MessageRepository(ZenBlogContext context) : base(context) { }
}
