using ZenBlog.Application.Features.UserAttributeFeatures.RoleFeatures.Commands.CreateRole;
using ZenBlog.Application.Features.UserAttributeFeatures.RoleFeatures.Commands.DeleteRole;
using ZenBlog.Domain.Entities.UserEntities;

namespace ZenBlog.Application.Services.UserAttributeService;

public interface IRoleService
{
    IQueryable<Role> GetAllRoles();
    Task CreateAsync(CreateRoleCommand request);
    Task DeleteAsync(DeleteRoleCommand request);
}
