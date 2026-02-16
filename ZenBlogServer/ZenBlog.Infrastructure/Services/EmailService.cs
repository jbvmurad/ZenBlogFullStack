using FluentEmail.Core;
using FluentEmail.Smtp;
using System.Net;
using System.Net.Mail;
using ZenBlog.Application.Services.MailService;
using ZenBlog.Domain.DTOs.SystemDTOs;
using ZenBlog.Domain.DTOs.UserDTOs;

public class EmailService : IEmailService
{
    private readonly EmailParameters _settings;
    private readonly ClientAppUrls _clientAppUrls;

    public EmailService(EmailParameters settings,ClientAppUrls clientAppUrls)
    {
        _settings = settings;
        _clientAppUrls = clientAppUrls;

        var sender = new SmtpSender(() => new SmtpClient(_settings.Host)
        {
            EnableSsl = _settings.EnableSsl,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            Port = _settings.Port,
            Credentials = new NetworkCredential(_settings.Username, _settings.AppPassword)
        });

        Email.DefaultSender = sender;
    }


    public async Task<bool> SendEmailAsync(string to, string subject, string body)
    {
        var response = await Email
            .From(_settings.From)
            .To(to)
            .Subject(subject)
            .Body(body, isHtml: true)
            .SendAsync();

        return response.Successful;
    }

    public async Task<bool> SendVerificationEmailAsync(string to, string userId, string token)
    {
        var verificationLink = BuildClientUrl(
            _clientAppUrls.VerifyEmailPath,
            userId,
            token);

        var body = $@"
        <h2>Welcome!</h2>
        <p>Please click the link below to verify your account:</p>
        <p><a href=""{verificationLink}"">Verify My Account</a></p>
        <p>If the button doesn't work, copy this link:</p>
        <p>{verificationLink}</p>
        ";

        return await SendEmailAsync(to, "Verify Your Account", body);
    }


    public async Task<bool> SendPasswordResetEmailAsync(string to, string userId, string token)
    {
        var resetLink = BuildClientUrl(
            _clientAppUrls.ResetPasswordPath,
            userId,
            token);

        var body = $@"
        <h2>Reset Password</h2>
        <p>Click the link below to reset your password:</p>
        <p><a href=""{resetLink}"">Reset Password</a></p>
        <p>If the button doesn't work, copy this link:</p>
        <p>{resetLink}</p>
        ";

        return await SendEmailAsync(to, "Reset Password", body);
    }


    public async Task<bool> SendDeletionNotificationEmailAsync(string to)
    {
        var body = @"
           <h2>Your Account Has Been Deleted</h2>
           <p>Your account has been successfully deleted. Thank you for using our service.</p>
        ";

        return await SendEmailAsync(to, "Account Deletion Notice", body);
    }

    private string BuildClientUrl(string path, string userId, string token)
    {
        var baseUrl = (_clientAppUrls.BaseUrl ?? string.Empty).TrimEnd('/');
        var cleanPath = (path ?? string.Empty).TrimStart('/');
        var encodedUserId = Uri.EscapeDataString(userId ?? string.Empty);
        var encodedToken = Uri.EscapeDataString(token ?? string.Empty);

        // Example: https://client.com/verify-email?userId=...&token=...
        return $"{baseUrl}/{cleanPath}?userId={encodedUserId}&token={encodedToken}";
    }
}
