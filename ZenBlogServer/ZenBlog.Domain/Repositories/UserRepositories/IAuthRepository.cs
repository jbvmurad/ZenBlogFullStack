using GenericRepository;
using ZenBlog.Domain.Entities.UserEntities;

namespace ZenBlog.Domain.Repositories.UserRepositories;

public interface IAuthRepository :IRepository<User> { }
