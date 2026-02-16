using FluentValidation;

namespace ZenBlog.Application.Features.ZenBlogFeatures.CommentFeatures.Commands.CreateComment;

public sealed class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
{
    public CreateCommentCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Body).NotEmpty();
        RuleFor(x => x.BlogId).NotEmpty();
    }
}
