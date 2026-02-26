using ZenBlog.Application.Features.UserAttributeFeatures.AuthFeatures.Commands.DeleteUser;
using ZenBlog.Application.Services.UserAttributeService;
using ZenBlog.Domain.DTOs.SystemDTOs;

namespace ZenBlog.Application.Features.UserAttributeFeatures.AuthFeatures.Commands.DeleteUser;

public class DeleteUserCommandHandler
{
    private readonly IAuthService _authService;

    public DeleteUserCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<MessageResponse> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        await _authService.DeleteAsync(request, cancellationToken);
        return new MessageResponse("User deleted successfully.");
    }
}
