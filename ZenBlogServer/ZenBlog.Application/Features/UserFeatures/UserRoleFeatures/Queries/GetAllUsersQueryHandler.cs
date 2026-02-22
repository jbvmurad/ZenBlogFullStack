using ZenBlog.Application.Services.UserAttributeService;
using ZenBlog.Domain.Entities.UserEntities;

namespace ZenBlog.Application.Features.UserAttributeFeatures.UserRoleFeatures.Queries;

public class GetAllUsersQueryHandler 
{
    private readonly IUserRoleService _userRoleService;

    public GetAllUsersQueryHandler(IUserRoleService userRoleService)
    {
        _userRoleService = userRoleService;
    }

    public Task<IQueryable<UserRole>> Handle(GetAllUserRolesQuery request, CancellationToken cancellationToken) =>
        Task.FromResult(_userRoleService.GetAllUserRoles());
}
