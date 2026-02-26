using ZenBlog.Application.Services.UserAttributeService;
using ZenBlog.Domain.DTOs.SystemDTOs;

namespace ZenBlog.Application.Features.UserAttributeFeatures.UserRoleFeatures.Commands.GiveUserRole;

public sealed class GiveUserRoleCommandHandler
{
    private readonly IUserRoleService _userRoleService;

    public GiveUserRoleCommandHandler(IUserRoleService userRoleService)
    {
        _userRoleService = userRoleService;
    }

    public async Task<MessageResponse> Handle(GiveUserRoleCommand request, CancellationToken cancellationToken)
    {
        await _userRoleService.GiveAsync(request, cancellationToken);
        return new MessageResponse("The role has been successfully assigned to the user");
    }
}
