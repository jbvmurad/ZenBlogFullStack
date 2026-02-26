using FluentValidation;

namespace ZenBlog.Application.Features.ZenBlogFeatures.SocialFeatures.Commands.UpdateSocialWithMedia;

public sealed class UpdateSocialWithMediaCommandValidator : AbstractValidator<UpdateSocialWithMediaCommand>
{
    public UpdateSocialWithMediaCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
