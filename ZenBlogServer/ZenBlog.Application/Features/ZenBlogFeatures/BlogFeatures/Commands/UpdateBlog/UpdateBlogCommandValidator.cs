using FluentValidation;

namespace ZenBlog.Application.Features.ZenBlogFeatures.BlogFeatures.Commands.UpdateBlog;

public sealed class UpdateBlogCommandValidator : AbstractValidator<UpdateBlogCommand>
{
    public UpdateBlogCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.CoverImage).NotEmpty();
        RuleFor(x => x.BlogImage).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}
