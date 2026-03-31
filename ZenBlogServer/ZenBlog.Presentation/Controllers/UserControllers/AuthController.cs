using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Wolverine;
using ZenBlog.Application.Features.UserFeatures.AuthFeatures.Commands.ConfirmEmail;
using ZenBlog.Application.Features.UserFeatures.AuthFeatures.Commands.CreateNewTokenByRefreshToken;
using ZenBlog.Application.Features.UserFeatures.AuthFeatures.Commands.DeleteUser;
using ZenBlog.Application.Features.UserFeatures.AuthFeatures.Commands.ForgotPassword;
using ZenBlog.Application.Features.UserFeatures.AuthFeatures.Commands.Login;
using ZenBlog.Application.Features.UserFeatures.AuthFeatures.Commands.LoginWithGoogle;
using ZenBlog.Application.Features.UserFeatures.AuthFeatures.Commands.RegisterUser;
using ZenBlog.Application.Features.UserFeatures.AuthFeatures.Commands.ResendEmailConfirmation;
using ZenBlog.Application.Features.UserFeatures.AuthFeatures.Commands.ResetPassword;
using ZenBlog.Application.Features.UserFeatures.AuthFeatures.Commands.UpdateUser;
using ZenBlog.Application.Services.UserAttributeService;
using ZenBlog.Domain.DTOs.SystemDTOs;
using ZenBlog.Domain.DTOs.UserDTOs;
using ZenBlog.Domain.Entities.UserEntities;
using ZenBlog.Presentation.Controllers.Abstraction;

namespace ZenBlog.Presentation.Controllers.UserControllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : APIController
{
    private readonly IAuthService _authService;
    private readonly IMapper _mapper;

    public AuthController(IMessageBus bus, IAuthService authService, IMapper mapper) : base(bus)
    {
        _authService = authService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(ODataQueryOptions<User> options, CancellationToken cancellationToken)
    {
        IQueryable<User> query = _authService.GetAllUsers();

        query = (IQueryable<User>)options.ApplyTo(query, new ODataQuerySettings());

        var result = await query
            .ProjectTo<UserResponse>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return Ok(result);
    }
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("confirm-email")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("resend-confirmation")]
    [AllowAnonymous]
    public async Task<IActionResult> ResendConfirmation(ResendEmailConfirmationCommand request, CancellationToken cancellationToken)
    {
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginCommand request, CancellationToken cancellationToken)
    {
        LoginCommandResponse response = await _bus.InvokeAsync<LoginCommandResponse>(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("login-google")]
    [AllowAnonymous]
    public async Task<IActionResult> LoginGoogle(LoginWithGoogleCommand request, CancellationToken cancellationToken)
    {
        LoginCommandResponse response = await _bus.InvokeAsync<LoginCommandResponse>(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("createtoken")]
    public async Task<IActionResult> CreateTokenByRefreshToken(CreateNewTokenByRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        LoginCommandResponse response = await _bus.InvokeAsync<LoginCommandResponse>(request, cancellationToken);
        return Ok(response);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        DeleteUserCommand request = new(id.ToString());
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request, cancellationToken);
        return Ok(response);
    }

    [HttpPut]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Update([FromForm] UpdateUserCommand request, CancellationToken cancellationToken)
    {
        MessageResponse response = await _bus.InvokeAsync<MessageResponse>(request, cancellationToken);
        return Ok(response);
    }
}
