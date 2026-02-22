using FluentValidation;

namespace ZenBlog.Application.Features.ZenBlogFeatures.BlogFeatures.Commands.CreateBlogWithMedia;

public sealed class CreateBlogWithMediaCommandValidator : AbstractValidator<CreateBlogWithMediaCommand>
{
    public CreateBlogWithMediaCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.CoverImage).NotNull();
        RuleFor(x => x.BlogImage).NotNull();
    }
}
