using FluentValidation;

namespace ZenBlog.Application.Features.ZenBlogFeatures.ContactInfoFeatures.Commands.DeleteContactInfo;

public sealed class DeleteContactInfoCommandValidator : AbstractValidator<DeleteContactInfoCommand>
{
    public DeleteContactInfoCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
