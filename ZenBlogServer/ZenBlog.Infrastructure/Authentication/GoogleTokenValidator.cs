using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using ZenBlog.Application.Services.ExternalAuth;
using ZenBlog.Domain.DTOs.UserDTOs;

namespace ZenBlog.Infrastructure.Authentication;

public sealed class GoogleTokenValidator : IGoogleTokenValidator
{
    private readonly string _googleClientId;
    private readonly GoogleAuthOptions _googleAuthOptions;

    public GoogleTokenValidator(IConfiguration configuration, GoogleAuthOptions googleAuthOptions)
    {
        _googleAuthOptions = googleAuthOptions ?? throw new ArgumentNullException(nameof(googleAuthOptions));

        _googleClientId = configuration["GoogleAuth:ClientId"]
           ?? throw new InvalidOperationException("GoogleAuth:ClientId is missing in configuration.");

        if (string.IsNullOrWhiteSpace(_googleAuthOptions.ClientId))
            throw new InvalidOperationException("GoogleAuthOptions.ClientId is not configured.");

        if (!string.Equals(_googleAuthOptions.ClientId, _googleClientId, StringComparison.Ordinal))
            throw new InvalidOperationException("GoogleAuthOptions.ClientId and GoogleAuth:ClientId must match.");
    }

    public async Task<GoogleUserInfo> ValidateAsync(string idToken, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(idToken))
            throw new ArgumentException("ID token must not be null or empty.", nameof(idToken));

        if (string.IsNullOrEmpty(_googleClientId))
            throw new InvalidOperationException("Google Client ID is not configured.");

        var payload = await GoogleJsonWebSignature.ValidateAsync(
            idToken,
            new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _googleClientId }
            }
        );

        if (string.IsNullOrWhiteSpace(payload.Email))
            throw new ArgumentException("Google token is invalid");

        var fullName = payload.Name;
        if (string.IsNullOrWhiteSpace(fullName)) fullName = payload.Email;

        return new GoogleUserInfo(payload.Email, fullName, payload.Subject);
    }
}
