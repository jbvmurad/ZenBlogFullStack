using FluentValidation;

namespace ZenBlog.Application.Features.ZenBlogFeatures.SocialFeatures.Commands.CreateSocialWithMedia;

public sealed class CreateSocialWithMediaCommandValidator : AbstractValidator<CreateSocialWithMediaCommand>
{
    public CreateSocialWithMediaCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Url).NotEmpty();
        RuleFor(x => x.Icon).NotNull();
    }
}
