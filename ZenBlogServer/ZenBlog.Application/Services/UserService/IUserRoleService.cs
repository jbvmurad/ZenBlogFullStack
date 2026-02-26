using ZenBlog.Application.Features.UserAttributeFeatures.UserRoleFeatures.Commands.DeleteUserRole;
using ZenBlog.Application.Features.UserAttributeFeatures.UserRoleFeatures.Commands.GiveUserRole;
using ZenBlog.Domain.Entities.UserEntities;

namespace ZenBlog.Application.Services.UserAttributeService;

public interface IUserRoleService
{
    Task GiveAsync(GiveUserRoleCommand request, CancellationToken cancellationToken);
    Task DeleteAsync(DeleteUserRoleFullCommand request, CancellationToken cancellationToken);
    IQueryable<UserRole> GetAllUserRoles();
}
