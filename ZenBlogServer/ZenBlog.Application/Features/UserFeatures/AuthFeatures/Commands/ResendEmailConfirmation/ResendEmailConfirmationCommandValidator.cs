using FluentValidation;
using ZenBlog.Application.Features.UserAttributeFeatures.AuthFeatures.Commands.ResendEmailConfirmation;

namespace ZenBlog.Application.Features.UserAttributeFeatures.AuthFeatures.Commands.ResendEmailConfirmation;

public sealed class ResendEmailConfirmationCommandValidator : AbstractValidator<ResendEmailConfirmationCommand>
{
    public ResendEmailConfirmationCommandValidator()
    {
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email is not valid.")
            .Length(5, 254).WithMessage("Email must be between 5 and 254 characters.");
    }
}
