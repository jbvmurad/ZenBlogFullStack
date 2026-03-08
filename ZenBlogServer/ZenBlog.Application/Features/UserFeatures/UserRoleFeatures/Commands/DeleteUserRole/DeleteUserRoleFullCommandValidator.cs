using FluentValidation;

namespace ZenBlog.Application.Features.UserFeatures.UserRoleFeatures.Commands.DeleteUserRole;

public sealed class DeleteUserRoleFullCommandValidator : AbstractValidator<DeleteUserRoleFullCommand>
{
    public DeleteUserRoleFullCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required.");

        RuleFor(x => x.RoleIds)
            .NotEmpty()
            .WithMessage("At least one RoleId must be provided.");

        RuleForEach(x => x.RoleIds)
            .NotEmpty()
            .WithMessage("Each RoleId is required.");
    }
}
