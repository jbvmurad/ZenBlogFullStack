using FluentValidation;

namespace ZenBlog.Application.Features.UserAttributeFeatures.AuthFeatures.Commands.ForgotPassword;

public sealed class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email is not valid.")
            .Length(5, 254).WithMessage("Email must be between 5 and 254 characters.");
    }
}
