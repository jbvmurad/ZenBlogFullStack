using FluentValidation;

namespace ZenBlog.Application.Features.UserAttributeFeatures.AuthFeatures.Commands.RegisterUser;

public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.FullName)
             .NotEmpty().WithMessage("Full Name is required.")
             .Length(2, 300).WithMessage("First name must be between 2 and 300 characters.");



        RuleFor(x => x.Email)
             .Cascade(CascadeMode.Stop)
             .NotEmpty().WithMessage("Email is required.")
             .Matches(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")
             .WithMessage("Email format must be: user@domain.com")
             .EmailAddress().WithMessage("Email is not valid.")
             .Length(5, 254).WithMessage("Email must be between 5 and 254 characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");
    }
}
