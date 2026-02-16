using ZenBlog.Application.Features.UserAttributeFeatures.AuthFeatures.Commands.Login;
using ZenBlog.Application.Services.UserAttributeService;

namespace ZenBlog.Application.Features.UserAttributeFeatures.AuthFeatures.Commands.CreateNewTokenByRefreshToken;

public sealed class CreateNewTokenByRefreshTokenCommandHandler 
{
    private readonly IAuthService _authService;
    public CreateNewTokenByRefreshTokenCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }
    public async Task<LoginCommandResponse> Handle(CreateNewTokenByRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        LoginCommandResponse response = await _authService.CreateTokenByRefreshTokenAsync(request, cancellationToken);
        return response;
    }
}
