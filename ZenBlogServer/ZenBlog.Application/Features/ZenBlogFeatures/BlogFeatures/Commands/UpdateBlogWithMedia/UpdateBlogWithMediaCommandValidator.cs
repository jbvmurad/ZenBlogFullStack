using FluentValidation;

namespace ZenBlog.Application.Features.ZenBlogFeatures.BlogFeatures.Commands.UpdateBlogWithMedia;

public sealed class UpdateBlogWithMediaCommandValidator : AbstractValidator<UpdateBlogWithMediaCommand>
{
    public UpdateBlogWithMediaCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
