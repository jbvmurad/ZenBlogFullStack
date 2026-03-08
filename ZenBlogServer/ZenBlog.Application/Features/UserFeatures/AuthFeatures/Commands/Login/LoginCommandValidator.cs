using FluentValidation;

namespace ZenBlog.Application.Features.UserFeatures.AuthFeatures.Commands.Login;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Email  is required.");
    }
}
