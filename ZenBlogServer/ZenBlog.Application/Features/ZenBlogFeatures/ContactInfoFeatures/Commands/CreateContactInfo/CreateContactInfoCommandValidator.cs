using FluentValidation;

namespace ZenBlog.Application.Features.ZenBlogFeatures.ContactInfoFeatures.Commands.CreateContactInfo;

public sealed class CreateContactInfoCommandValidator : AbstractValidator<CreateContactInfoCommand>
{
    public CreateContactInfoCommandValidator()
    {
        RuleFor(x => x.Address).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Phone).NotEmpty();
        RuleFor(x => x.MapUrl).NotEmpty();
    }
}
