using ZenBlog.Application.Features.UserAttributeFeatures.RoleFeatures.Queries;
using ZenBlog.Application.Services.UserAttributeService;
using ZenBlog.Domain.Entities.UserEntities;

public sealed class GetAllRolesQueryHandler 
{
    private readonly IRoleService _roleService;

    public GetAllRolesQueryHandler(IRoleService roleService) => _roleService = roleService;

    public Task<IQueryable<Role>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        => Task.FromResult(_roleService.GetAllRoles());
}
