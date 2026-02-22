using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
using ZenBlog.Application.Jwt;
using ZenBlog.Application.Services.ExternalAuth;
using ZenBlog.Application.Services.MailService;
using ZenBlog.Application.Services.UserAttributeService;
using ZenBlog.Domain.Entities.UserEntities;
using ZenBlog.Domain.Repositories.UserRepositories;

namespace ZenBlog.Persistance.Services.UserServices;

public sealed class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IAuthRepository _authRepository;
    private readonly IJwtProvider _jwtProvider;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;
    private readonly IGoogleTokenValidator _googleTokenValidator;

    public AuthService(
        UserManager<User> userManager,
        IMapper mapper,
        IEmailService emailService,
        IAuthRepository authRepository,
        IJwtProvider jwtProvider,
        IGoogleTokenValidator googleTokenValidator)
    {
        _userManager = userManager;
        _mapper = mapper;
        _authRepository = authRepository;
        _jwtProvider = jwtProvider;
        _emailService = emailService;
        _googleTokenValidator = googleTokenValidator;
    }

    public async Task RegisterAsync(RegisterUserCommand request)
    {
        User user = _mapper.Map<User>(request);
        user.Email = request.Email;
        user.UserName = request.Email;
        IdentityResult result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            throw new Exception(result.Errors.First().Description);
        }
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        await _emailService.SendVerificationEmailAsync(user.Email, user.Id.ToString(), token);
    }

    public async Task ConfirmEmailAsync(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user is null) throw new ArgumentException("User not found");

        var decodedToken = Uri.UnescapeDataString(request.Token);
        var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
        if (!result.Succeeded)
            throw new Exception(result.Errors.First().Description);
    }

    public async Task ResendEmailConfirmationAsync(ResendEmailConfirmationCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null) throw new ArgumentException("User not found");
        if (user.EmailConfirmed) return;

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        await _emailService.SendVerificationEmailAsync(user.Email!, user.Id.ToString(), token);
    }

    public async Task ForgotPasswordAsync(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null) throw new ArgumentException("User not found");

        if (!user.EmailConfirmed) throw new ArgumentException("Please verify your email before resetting password.");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        await _emailService.SendPasswordResetEmailAsync(user.Email!, user.Id.ToString(), token);
    }

    public async Task ResetPasswordAsync(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user is null) throw new ArgumentException("User not found");

        var decodedToken = Uri.UnescapeDataString(request.Token);
        var result = await _userManager.ResetPasswordAsync(user, decodedToken, request.NewPassword);
        if (!result.Succeeded)
            throw new Exception(result.Errors.First().Description);
    }


    public async Task UpdateAsync(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id.ToString());

        if (user is null)
            throw new ArgumentException($"User with ID {request.Id} not found.");

        _mapper.Map(request, user);

        if (!string.IsNullOrEmpty(request.Password))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, request.Password);

            if (!result.Succeeded)
                throw new Exception(result.Errors.First().Description);
        }

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            throw new Exception(updateResult.Errors.First().Description);
    }

    public async Task DeleteAsync(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id.ToString());
        if (user is null)
            throw new ArgumentException($"User with ID {request.Id} not found.");

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
            throw new Exception(result.Errors.First().Description);
        await _emailService.SendDeletionNotificationEmailAsync(user.Email);
    }

    public async Task<LoginCommandResponse> LoginAsync(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null) throw new ArgumentException("User not found");

        if (!user.EmailConfirmed)
            throw new ArgumentException("Please verify your email before logging in.");

        var result = await _userManager.CheckPasswordAsync(user, request.Password);
        if (result)
        {
            LoginCommandResponse response = await _jwtProvider.CreateTokenAsync(user);
            return response;
        }

        throw new ArgumentException("Incorrect password entered.");
    }

    public async Task<LoginCommandResponse> LoginWithGoogleAsync(LoginWithGoogleCommand request, CancellationToken cancellationToken)
    {
        var info = await _googleTokenValidator.ValidateAsync(request.IdToken, cancellationToken);

        var user = await _userManager.FindByEmailAsync(info.Email);
        if (user is null)
        {
            user = new User
            {
                Email = info.Email,
                UserName = info.Email,
                FullName = info.FullName,
                EmailConfirmed = true
            };

            var create = await _userManager.CreateAsync(user);
            if (!create.Succeeded)
                throw new Exception(create.Errors.First().Description);
        }
        else
        {
            user.EmailConfirmed = true;
            if (!string.IsNullOrWhiteSpace(info.FullName) && user.FullName != info.FullName)
                user.FullName = info.FullName;
            await _userManager.UpdateAsync(user);
        }

        return await _jwtProvider.CreateTokenAsync(user);
    }

    public async Task<LoginCommandResponse> CreateTokenByRefreshTokenAsync(CreateNewTokenByRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        User user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null) throw new ArgumentException("User not found");
        if (user.RefreshToken != request.RefreshToken) throw new ArgumentException("RefreshToken is invalid.");
        if (user.RefreshTokenExpires < DateTime.UtcNow) throw new ArgumentException("RefreshToken is expired");
        LoginCommandResponse response = await _jwtProvider.CreateTokenAsync(user);
        return response;
    }

    public IQueryable<User> GetAllUsers() => _userManager.Users.AsNoTracking();
}
