using FluentValidation;
using ZenBlog.Application.Features.UserFeatures.AuthFeatures.Commands.DeleteUser;

namespace ZenBlog.Application.Features.UserFeatures.AuthFeatures.Commands.DeleteUser;

public sealed class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(x => x.Id)
           .NotEmpty().WithMessage("Product ID is required.");
    }
}
