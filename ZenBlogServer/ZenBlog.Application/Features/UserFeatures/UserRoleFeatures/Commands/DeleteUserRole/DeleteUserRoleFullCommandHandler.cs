using ZenBlog.Application.Services.UserAttributeService;
using ZenBlog.Domain.DTOs.SystemDTOs;

namespace ZenBlog.Application.Features.UserAttributeFeatures.UserRoleFeatures.Commands.DeleteUserRole;

public sealed class DeleteUserRoleFullCommandHandler 
{
    private readonly IUserRoleService _userRoleService;

    public DeleteUserRoleFullCommandHandler(IUserRoleService userRoleService)
    {
        _userRoleService = userRoleService;
    }

    public async Task<MessageResponse> Handle(DeleteUserRoleFullCommand request, CancellationToken cancellationToken)
    {
        await _userRoleService.DeleteAsync(request, cancellationToken);
        return new MessageResponse("Selected roles have been removed from the user.");
    }
}

