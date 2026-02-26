using ZenBlog.Application.Services.UserAttributeService;
using ZenBlog.Domain.DTOs.SystemDTOs;

namespace ZenBlog.Application.Features.UserAttributeFeatures.AuthFeatures.Commands.UpdateUser;

public sealed class UpdateUserCommandHandler
{
    private readonly IAuthService _authService;

    public UpdateUserCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<MessageResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        await _authService.UpdateAsync(request, cancellationToken);
        return new MessageResponse("User updated successfully.");
    }
}
