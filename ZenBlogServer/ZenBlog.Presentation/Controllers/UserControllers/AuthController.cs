using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Wolverine;
using ZenBlog.Application.Features.UserAttributeFeatures.AuthFeatures.Commands.ConfirmEmail;
using ZenBlog.Application.Features.UserAttributeFeatures.AuthFeatures.Commands.CreateNewTokenByRefreshToken;
using ZenBlog.Application.Features.UserAttributeFeatures.AuthFeatures.Commands.DeleteUser;
using ZenBlog.Application.Features.UserAttributeFeatures.AuthFeatures.Commands.ForgotPassword;
using ZenBlog.Application.Features.UserAttributeFeatures.AuthFeatures.Commands.Login;
using ZenBlog.Application.Features.UserAttributeFeatures.AuthFeatures.Commands.LoginWithGoogle;
using ZenBlog.Application.Features.UserAttributeFeatures.AuthFeatures.Commands.RegisterUser;
using ZenBlog.Application.Features.UserAttributeFeatures.AuthFeatures.Commands.ResendEmailConfirmation;
using ZenBlog.Application.Features.UserAttributeFeatures.AuthFeatures.Commands.ResetPassword;
using ZenBlog.Application.Features.UserAttributeFeatures.AuthFeatures.Commands.UpdateUser;
using ZenBlog.Application.Services.UserAttributeService;
using ZenBlog.Domain.DTOs.SystemDTOs;
using ZenBlog.Domain.Entities.UserEntities;
using ZenBlog.Presentation.Controllers.Abstraction;

namespace ZenBlog.Presentation.Controllers.UserControllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : APIController
{
    private readonly IAuthService _authService;

    public AuthController(IMessageBus bus, IAuthService authService) : base(bus)
    {
        _authService = authService;
    }

    [HttpGet]
    [EnableQuery]
    public IQueryable<User> GetAll() => _authService.GetAllUsers();


    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request);
        return Ok(response);
    }

    [HttpPost("confirm-email")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request);
        return Ok(response);
    }

    [HttpPost("resend-confirmation")]
    [AllowAnonymous]
    public async Task<IActionResult> ResendConfirmation(ResendEmailConfirmationCommand request, CancellationToken cancellationToken)
    {
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request);
        return Ok(response);
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request);
        return Ok(response);
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request);
        return Ok(response);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginCommand request, CancellationToken cancellationToken)
    {
        LoginCommandResponse response = await _bus.InvokeAsync<LoginCommandResponse>(request);
        return Ok(response);
    }

    [HttpPost("login-google")]
    [AllowAnonymous]
    public async Task<IActionResult> LoginGoogle(LoginWithGoogleCommand request, CancellationToken cancellationToken)
    {
        LoginCommandResponse response = await _bus.InvokeAsync<LoginCommandResponse>(request);
        return Ok(response);
    }

    [HttpPost("createtoken")]
    public async Task<IActionResult> CreateTokenByRefreshToken(CreateNewTokenByRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        LoginCommandResponse response = await _bus.InvokeAsync<LoginCommandResponse>(request);
        return Ok(response);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        DeleteUserCommand request = new(id.ToString());
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request);
        return Ok(response);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request);
        return Ok(response);
    }
}
