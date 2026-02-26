using GenericRepository;
using ZenBlog.Domain.Entities.UserEntities;
using ZenBlog.Domain.Repositories.UserRepositories;
using ZenBlog.Persistance.Context;

namespace ZenBlog.Persistance.Repositories.UserRepositories
{
    public sealed class AuthRepository : Repository<User, ZenBlogContext>, IAuthRepository
    {
        public AuthRepository(ZenBlogContext context) : base(context) { }
    }
}
