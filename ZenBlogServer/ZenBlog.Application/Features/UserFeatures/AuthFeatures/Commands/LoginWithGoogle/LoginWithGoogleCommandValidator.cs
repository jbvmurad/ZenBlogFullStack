using FluentValidation;

namespace ZenBlog.Application.Features.UserAttributeFeatures.AuthFeatures.Commands.LoginWithGoogle;

public sealed class LoginWithGoogleCommandValidator : AbstractValidator<LoginWithGoogleCommand>
{
    public LoginWithGoogleCommandValidator()
    {
        RuleFor(x => x.IdToken)
            .NotEmpty().WithMessage("IdToken is required.");
    }
}
