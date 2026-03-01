using ZenBlog.Application.Services.MailService;
using ZenBlog.Domain.Events;

namespace ZenBlog.API.Messaging.Handlers;
public sealed class AccountDeletedHandler
{
    private readonly IEmailService _emailService;
    private readonly ILogger<AccountDeletedHandler> _logger;

    public AccountDeletedHandler(IEmailService emailService, ILogger<AccountDeletedHandler> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Handle(AccountDeletedIntegrationEvent message)
    {
        var ok = await _emailService.SendDeletionNotificationEmailAsync(message.Email);
        if (!ok)
            throw new InvalidOperationException($"Failed to send account deletion email. UserId={message.UserId}");

        _logger.LogInformation("Account deletion email sent. UserId={UserId}, Email={Email}", message.UserId, message.Email);
    }
}
