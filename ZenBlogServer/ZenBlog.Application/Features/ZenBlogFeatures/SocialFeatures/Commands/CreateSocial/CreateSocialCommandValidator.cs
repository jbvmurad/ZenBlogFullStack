using FluentValidation;

namespace ZenBlog.Application.Features.ZenBlogFeatures.SocialFeatures.Commands.CreateSocial;

public sealed class CreateSocialCommandValidator : AbstractValidator<CreateSocialCommand>
{
    public CreateSocialCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Url).NotEmpty();
        RuleFor(x => x.Icon).NotEmpty();
    }
}
