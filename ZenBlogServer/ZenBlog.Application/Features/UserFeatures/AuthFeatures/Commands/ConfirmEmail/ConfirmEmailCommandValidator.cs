using FluentValidation;
using ZenBlog.Application.Features.UserAttributeFeatures.AuthFeatures.Commands.ConfirmEmail;

namespace ZenBlog.Application.Features.UserAttributeFeatures.AuthFeatures.Commands.ConfirmEmail;

public sealed class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token is required.");
    }
}
