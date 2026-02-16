using FluentValidation;

namespace ZenBlog.Application.Features.ZenBlogFeatures.SocialFeatures.Commands.DeleteSocial;

public sealed class DeleteSocialCommandValidator : AbstractValidator<DeleteSocialCommand>
{
    public DeleteSocialCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
