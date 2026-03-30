using FluentValidation;

namespace ZenBlog.Application.Features.ZenBlogFeatures.SocialFeatures.Commands.UpdateSocial;

public sealed class UpdateSocialCommandValidator : AbstractValidator<UpdateSocialCommand>
{
    public UpdateSocialCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
