using FluentValidation;

namespace ZenBlog.Application.Features.UserAttributeFeatures.AuthFeatures.Commands.CreateNewTokenByRefreshToken;

public sealed class CreateNewTokenByRefreshTokenCommandValidator : AbstractValidator<CreateNewTokenByRefreshTokenCommand>
{
    public CreateNewTokenByRefreshTokenCommandValidator()
    {
        RuleFor(x => x.UserId)
           .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.RefreshToken)
        .NotEmpty().WithMessage("RefreshToken is required.");
    }
}
