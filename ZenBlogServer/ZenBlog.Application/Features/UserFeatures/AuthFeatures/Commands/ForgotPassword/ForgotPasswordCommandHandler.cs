using ZenBlog.Application.Services.UserAttributeService;
using ZenBlog.Domain.DTOs.SystemDTOs;

namespace ZenBlog.Application.Features.UserAttributeFeatures.AuthFeatures.Commands.ForgotPassword;

public sealed class ForgotPasswordCommandHandler
{
    private readonly IAuthService _authService;

    public ForgotPasswordCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<MessageResponse> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        await _authService.ForgotPasswordAsync(request, cancellationToken);
        return new MessageResponse("If the account exists, a password reset email has been sent.");
    }
}
