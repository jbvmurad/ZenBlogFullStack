using ZenBlog.Application.Features.UserAttributeFeatures.AuthFeatures.Commands.Login;
using ZenBlog.Application.Services.UserAttributeService;

namespace ZenBlog.Application.Features.UserAttributeFeatures.AuthFeatures.Commands.LoginWithGoogle;

public sealed class LoginWithGoogleCommandHandler 
{
    private readonly IAuthService _authService;

    public LoginWithGoogleCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public Task<LoginCommandResponse> Handle(LoginWithGoogleCommand request, CancellationToken cancellationToken)
        => _authService.LoginWithGoogleAsync(request, cancellationToken);
}
