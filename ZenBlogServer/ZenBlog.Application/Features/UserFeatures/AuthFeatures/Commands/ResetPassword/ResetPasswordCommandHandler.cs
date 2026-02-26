using ZenBlog.Application.Services.UserAttributeService;
using ZenBlog.Domain.DTOs.SystemDTOs;

namespace ZenBlog.Application.Features.UserAttributeFeatures.AuthFeatures.Commands.ResetPassword;

public sealed class ResetPasswordCommandHandler
{
    private readonly IAuthService _authService;

    public ResetPasswordCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<MessageResponse> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        await _authService.ResetPasswordAsync(request, cancellationToken);
        return new MessageResponse("Password has been reset successfully");
    }
}
