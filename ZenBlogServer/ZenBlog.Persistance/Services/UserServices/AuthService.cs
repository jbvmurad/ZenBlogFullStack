using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Wolverine;
using ZenBlog.Application.Features.UserFeatures.AuthFeatures.Commands.ConfirmEmail;
using ZenBlog.Application.Features.UserFeatures.AuthFeatures.Commands.CreateNewTokenByRefreshToken;
using ZenBlog.Application.Features.UserFeatures.AuthFeatures.Commands.DeleteUser;
using ZenBlog.Application.Features.UserFeatures.AuthFeatures.Commands.ForgotPassword;
using ZenBlog.Application.Features.UserFeatures.AuthFeatures.Commands.Login;
using ZenBlog.Application.Features.UserFeatures.AuthFeatures.Commands.LoginWithGoogle;
using ZenBlog.Application.Features.UserFeatures.AuthFeatures.Commands.Logout;
using ZenBlog.Application.Features.UserFeatures.AuthFeatures.Commands.RegisterUser;
using ZenBlog.Application.Features.UserFeatures.AuthFeatures.Commands.ResendEmailConfirmation;
using ZenBlog.Application.Features.UserFeatures.AuthFeatures.Commands.ResetPassword;
using ZenBlog.Application.Features.UserFeatures.AuthFeatures.Commands.UpdateUser;
using ZenBlog.Application.Jwt;
using ZenBlog.Application.Services.ExternalAuth;
using ZenBlog.Application.Services.Storage;
using ZenBlog.Application.Services.UserAttributeService;
using ZenBlog.Domain.Entities.UserEntities;
using ZenBlog.Domain.Events;

namespace ZenBlog.Persistance.Services.UserServices;

public sealed class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtProvider _jwtProvider;
    private readonly IMapper _mapper;
    private readonly IGoogleTokenValidator _googleTokenValidator;
    private readonly IMessageBus _bus;
    private readonly IFileStorage _fileStorage;

    public AuthService(
        UserManager<User> userManager,
        IMapper mapper,
        IJwtProvider jwtProvider,
        IGoogleTokenValidator googleTokenValidator,
        IMessageBus bus,
        IFileStorage fileStorage)
    {
        _userManager = userManager;
        _mapper = mapper;
        _jwtProvider = jwtProvider;
        _googleTokenValidator = googleTokenValidator;
        _bus = bus;
        _fileStorage = fileStorage;
    }

    public async Task<string> SaveUserImageAsync(IFormFile media, CancellationToken cancellationToken)
    {
        return await _fileStorage.SaveImageAsync(media, cancellationToken);
    }

    public async Task RegisterAsync(RegisterUserCommand request)
    {
        User user = _mapper.Map<User>(request);
        user.Email = request.Email;
        user.UserName = request.Email;
        IdentityResult result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            throw new ArgumentException(result.Errors.First().Description);
        }
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        await _bus.PublishAsync(new EmailConfirmationRequestedIntegrationEvent(
            UserId: user.Id.ToString(),
            Email: user.Email,
            Token: token,
            RequestedAtUtc: DateTime.UtcNow));
    }

    public async Task ConfirmEmailAsync(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user is null) throw new ArgumentException("User not found");

        var decodedToken = Uri.UnescapeDataString(request.Token);
        var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
        if (!result.Succeeded)
            throw new ArgumentException(result.Errors.First().Description);
    }

    public async Task ResendEmailConfirmationAsync(ResendEmailConfirmationCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null) throw new ArgumentException("User not found");
        if (user.EmailConfirmed) return;

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        await _bus.PublishAsync(new EmailConfirmationRequestedIntegrationEvent(
            UserId: user.Id.ToString(),
            Email: user.Email!,
            Token: token,
            RequestedAtUtc: DateTime.UtcNow));
    }

    public async Task ForgotPasswordAsync(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null) throw new ArgumentException("User not found");

        if (!user.EmailConfirmed) throw new ArgumentException("Please verify your email before resetting password.");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        await _bus.PublishAsync(new PasswordResetRequestedIntegrationEvent(
            UserId: user.Id.ToString(),
            Email: user.Email!,
            Token: token,
            RequestedAtUtc: DateTime.UtcNow));
    }

    public async Task ResetPasswordAsync(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user is null) throw new ArgumentException("User not found");

        var decodedToken = Uri.UnescapeDataString(request.Token);
        var result = await _userManager.ResetPasswordAsync(user, decodedToken, request.NewPassword);
        if (!result.Succeeded)
            throw new ArgumentException(result.Errors.First().Description);
    }


    public async Task UpdateAsync(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id.ToString());

        if (user is null)
            throw new ArgumentException($"User with ID {request.Id} not found.");

        var oldFullName = user.FullName;
        var oldEmail = user.Email;
        var oldPhone = user.PhoneNumber;
        var oldImageUrl = user.ImageUrl;
        string? uploadedImageUrl = null;

        if (!string.IsNullOrWhiteSpace(request.FullName))
            user.FullName = request.FullName;
        else
            user.FullName = oldFullName;

        if (!string.IsNullOrWhiteSpace(request.Email))
            user.Email = request.Email;
        else
            user.Email = oldEmail;

        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            user.PhoneNumber = request.PhoneNumber;
        else
            user.PhoneNumber = oldPhone;

        if (!string.IsNullOrWhiteSpace(user.Email) && !string.Equals(user.UserName, user.Email, StringComparison.OrdinalIgnoreCase))
            user.UserName = user.Email;

        try
        {
            if (request.Image is not null)
            {
                uploadedImageUrl = await _fileStorage.SaveImageAsync(request.Image, cancellationToken);
                user.ImageUrl = uploadedImageUrl;
            }
            else if (request.RemoveImage)
            {
                user.ImageUrl = null;
            }
            else
            {
                user.ImageUrl = oldImageUrl;
            }

            if (!string.IsNullOrEmpty(request.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, request.Password);

                if (!result.Succeeded)
                    throw new ArgumentException(result.Errors.First().Description);
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                throw new ArgumentException(updateResult.Errors.First().Description);
        }
        catch
        {
            if (!string.IsNullOrWhiteSpace(uploadedImageUrl) &&
                !string.Equals(uploadedImageUrl, oldImageUrl, StringComparison.OrdinalIgnoreCase))
            {
                await _fileStorage.TryDeleteAsync(uploadedImageUrl, cancellationToken);
            }

            throw;
        }

        if (!string.IsNullOrWhiteSpace(oldImageUrl) &&
            !string.Equals(oldImageUrl, user.ImageUrl, StringComparison.OrdinalIgnoreCase))
        {
            await _fileStorage.TryDeleteAsync(oldImageUrl, cancellationToken);
        }
    }

    public async Task DeleteAsync(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id.ToString());
        if (user is null)
            throw new ArgumentException($"User with ID {request.Id} not found.");

        var imageUrl = user.ImageUrl;

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
            throw new ArgumentException(result.Errors.First().Description);

        if (!string.IsNullOrWhiteSpace(imageUrl))
            await _fileStorage.TryDeleteAsync(imageUrl, cancellationToken);

        await _bus.PublishAsync(new AccountDeletedIntegrationEvent(
            UserId: user.Id.ToString(),
            Email: user.Email,
            DeletedAtUtc: DateTime.UtcNow));
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
                throw new ArgumentException(create.Errors.First().Description);
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

    public async Task LogoutAsync(LogoutCommand request, CancellationToken cancellationToken)
    {
        User? user = await _userManager.Users
            .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

        if (user is null)
            throw new ArgumentException($"User with Id {request.UserId} is not found");

        if (string.IsNullOrEmpty(user.RefreshToken))
            throw new ArgumentException("User does not have an active refresh token");

        if (user.RefreshToken != request.RefreshToken)
            throw new ArgumentException("RefreshToken is invalid");

        user.RefreshToken = null;
        user.RefreshTokenExpires = null;

        IdentityResult result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
            throw new Exception(string.Join(" | ", result.Errors.Select(s => s.Description)));
    }

    public IQueryable<User> GetAllUsers() => _userManager.Users.AsNoTracking();
}
