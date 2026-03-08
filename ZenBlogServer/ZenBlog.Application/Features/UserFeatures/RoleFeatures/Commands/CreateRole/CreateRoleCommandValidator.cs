using FluentValidation;

namespace ZenBlog.Application.Features.UserFeatures.RoleFeatures.Commands.CreateRole;

public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator()
    {
        RuleFor(x => x.Name)
          .NotEmpty().WithMessage("Role is required.");

        RuleFor(x => x.Name)
          .NotNull().WithMessage("Role is required.");
    }
}
