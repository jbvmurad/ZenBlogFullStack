using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ZenBlog.Application.Features.UserAttributeFeatures.RoleFeatures.Commands.CreateRole;
using ZenBlog.Application.Features.UserAttributeFeatures.RoleFeatures.Commands.DeleteRole;
using ZenBlog.Application.Services.UserAttributeService;
using ZenBlog.Domain.Entities.UserEntities;

namespace ZenBlog.Persistance.Services.UserServices;

public sealed class RoleService : IRoleService
{
    private readonly RoleManager<Role> _roleManager;

    public RoleService(RoleManager<Role> roleManager) => _roleManager = roleManager;

    public async Task CreateAsync(CreateRoleCommand request)
    {
        Role role = new()
        {
            Name = request.Name,
        };
        await _roleManager.CreateAsync(role);
    }

    public async Task DeleteAsync(DeleteRoleCommand request)
    {
        var role = await _roleManager.FindByIdAsync(request.Id.ToString());

        if (role is null)
            throw new ArgumentException($"Role with ID {request.Id} not found.");

        var result = await _roleManager.DeleteAsync(role);
    }

    public IQueryable<Role> GetAllRoles()
           => _roleManager.Roles.AsNoTracking();
}
