using ZenBlog.Application.Services.UserAttributeService;
using ZenBlog.Domain.DTOs.SystemDTOs;

namespace ZenBlog.Application.Features.UserFeatures.AuthFeatures.Commands.Logout;

public sealed class LogoutCommandHandler
{
    private readonly IAuthService _authService;

    public LogoutCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }
    
    public async Task<MessageResponse> Handle(LogoutCommand request,CancellationToken cancellationToken) 
    {
        await _authService.LogoutAsync(request, cancellationToken);
        return new MessageResponse("Logout successful");
    }
}
