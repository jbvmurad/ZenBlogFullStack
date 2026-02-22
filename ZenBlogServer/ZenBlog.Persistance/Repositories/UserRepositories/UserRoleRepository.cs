using GenericRepository;
using ZenBlog.Domain.Entities.UserEntities;
using ZenBlog.Domain.Repositories.UserRepositories;
using ZenBlog.Persistance.Context;

namespace ZenBlog.Persistance.Repositories.UserRepositories;

public sealed class UserRoleRepository : Repository<UserRole, ZenBlogContext>, IUserRoleRepository
{
    public UserRoleRepository(ZenBlogContext context) : base(context) { }
}
