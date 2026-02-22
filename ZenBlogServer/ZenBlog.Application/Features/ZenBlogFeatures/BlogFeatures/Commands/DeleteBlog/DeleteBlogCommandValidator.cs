using FluentValidation;

namespace ZenBlog.Application.Features.ZenBlogFeatures.BlogFeatures.Commands.DeleteBlog;

public sealed class DeleteBlogCommandValidator : AbstractValidator<DeleteBlogCommand>
{
    public DeleteBlogCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
