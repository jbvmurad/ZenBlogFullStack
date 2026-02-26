using FluentValidation;

namespace ZenBlog.Application.Features.UserAttributeFeatures.UserRoleFeatures.Commands.GiveUserRole;

public sealed class GiveUserRoleCommandValidator : AbstractValidator<GiveUserRoleCommand>
{
    public GiveUserRoleCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");
        RuleFor(x => x.UserId)
            .NotNull().WithMessage("UserId is required");

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("RoleId is required");

        RuleFor(x => x.RoleId)
            .NotNull().WithMessage("RoleId is required");
    }
}
