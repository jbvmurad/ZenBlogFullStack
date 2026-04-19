using FluentValidation;

namespace ZenBlog.Application.Features.UserFeatures.AuthFeatures.Commands.Logout;

public sealed class LogoutCommandValidator :AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("RefreshToken is required.");
    }
}
