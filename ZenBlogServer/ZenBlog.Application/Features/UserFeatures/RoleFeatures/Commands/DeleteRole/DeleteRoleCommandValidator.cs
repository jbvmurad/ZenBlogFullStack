using FluentValidation;

namespace ZenBlog.Application.Features.UserFeatures.RoleFeatures.Commands.DeleteRole;

public sealed class DeleteRoleCommandValidator : AbstractValidator<DeleteRoleCommand>
{
    public DeleteRoleCommandValidator()
    {
        RuleFor(x => x.Id)
           .NotEmpty().WithMessage("Id is required.");
        RuleFor(x => x.Id)
           .NotNull().WithMessage("Id is required.");
    }
}
