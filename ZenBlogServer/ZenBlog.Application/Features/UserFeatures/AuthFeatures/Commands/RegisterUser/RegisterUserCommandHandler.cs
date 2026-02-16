using ZenBlog.Application.Services.UserAttributeService;
using ZenBlog.Domain.DTOs.SystemDTOs;

namespace ZenBlog.Application.Features.UserAttributeFeatures.AuthFeatures.Commands.RegisterUser;

public sealed class RegisterUserCommandHandler 
{
    private readonly IAuthService _authService;

    public RegisterUserCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<MessageResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        await _authService.RegisterAsync(request);
        return new MessageResponse("User registered successfully");
    }
}

