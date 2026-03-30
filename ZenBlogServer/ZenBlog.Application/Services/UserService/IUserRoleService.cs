using ZenBlog.Application.Features.UserFeatures.UserRoleFeatures.Commands.DeleteUserRole;
using ZenBlog.Application.Features.UserFeatures.UserRoleFeatures.Commands.GiveUserRole;
using ZenBlog.Domain.DTOs.UserDTOs;
using ZenBlog.Domain.Entities.UserEntities;

namespace ZenBlog.Application.Services.UserAttributeService;

public interface IUserRoleService
{
    Task GiveAsync(GiveUserRoleCommand request, CancellationToken cancellationToken);
    Task DeleteAsync(DeleteUserRoleFullCommand request, CancellationToken cancellationToken);
    IQueryable<UserRoleResponse> GetAllUserRoles();
}
