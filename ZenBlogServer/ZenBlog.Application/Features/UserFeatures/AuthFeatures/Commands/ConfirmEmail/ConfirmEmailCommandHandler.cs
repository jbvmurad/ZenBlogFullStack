using ZenBlog.Application.Services.UserAttributeService;
using ZenBlog.Domain.DTOs.SystemDTOs;

namespace ZenBlog.Application.Features.UserFeatures.AuthFeatures.Commands.ConfirmEmail;

public sealed class ConfirmEmailCommandHandler
{
    private readonly IAuthService _authService;

    public ConfirmEmailCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<MessageResponse> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        await _authService.ConfirmEmailAsync(request, cancellationToken);
        return new MessageResponse("Email verified successfully");
    }
}
