using ZenBlog.Application.Services.UserAttributeService;
using ZenBlog.Domain.DTOs.SystemDTOs;

namespace ZenBlog.Application.Features.UserAttributeFeatures.AuthFeatures.Commands.ResendEmailConfirmation;

public sealed class ResendEmailConfirmationCommandHandler 
{
    private readonly IAuthService _authService;

    public ResendEmailConfirmationCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<MessageResponse> Handle(ResendEmailConfirmationCommand request, CancellationToken cancellationToken)
    {
        await _authService.ResendEmailConfirmationAsync(request, cancellationToken);
        return new MessageResponse("If the account exists, a verification email has been sent.");
    }
}
