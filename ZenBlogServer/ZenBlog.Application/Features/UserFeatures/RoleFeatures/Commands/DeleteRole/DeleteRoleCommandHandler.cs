using ZenBlog.Application.Features.UserAttributeFeatures.RoleFeatures.Commands.DeleteRole;
using ZenBlog.Application.Services.UserAttributeService;
using ZenBlog.Domain.DTOs.SystemDTOs;

namespace ZenBlog.Application.Features.UserAttributeFeatures.RoleFeatures.Commands.DeleteRole;

public class DeleteRoleCommandHandler
{
    private readonly IRoleService _roleService;

    public DeleteRoleCommandHandler(IRoleService roleService)
    {
        _roleService = roleService;
    }

    public async Task<MessageResponse> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        await _roleService.DeleteAsync(request);
        return new MessageResponse("Role deleted successfully");
    }
}
