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
using Microsoft.AspNetCore.Http;
using ZenBlog.Domain.Entities.UserEntities;

namespace ZenBlog.Application.Services.UserAttributeService;

public interface IAuthService
{
    Task RegisterAsync(RegisterUserCommand request);
    Task ConfirmEmailAsync(ConfirmEmailCommand request, CancellationToken cancellationToken);
    Task ResendEmailConfirmationAsync(ResendEmailConfirmationCommand request, CancellationToken cancellationToken);
    Task ForgotPasswordAsync(ForgotPasswordCommand request, CancellationToken cancellationToken);
    Task ResetPasswordAsync(ResetPasswordCommand request, CancellationToken cancellationToken);
    Task<LoginCommandResponse> LoginAsync(LoginCommand request, CancellationToken cancellationToken);
    Task<LoginCommandResponse> LoginWithGoogleAsync(LoginWithGoogleCommand request, CancellationToken cancellationToken);
    Task<LoginCommandResponse> CreateTokenByRefreshTokenAsync(
        CreateNewTokenByRefreshTokenCommand request, CancellationToken cancellationToken);
    IQueryable<User> GetAllUsers();
#nullable enable
    Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken);
#nullable disable
    Task UpdateAsync(UpdateUserCommand request, CancellationToken cancellationToken);
    Task DeleteAsync(DeleteUserCommand request, CancellationToken cancellationToken);
    Task<string> SaveUserImageAsync(IFormFile media, CancellationToken cancellationToken);
}
