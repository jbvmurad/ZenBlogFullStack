using FluentValidation;

namespace ZenBlog.Application.Features.ZenBlogFeatures.CommentFeatures.Commands.UpdateComment;

public sealed class UpdateCommentCommandValidator : AbstractValidator<UpdateCommentCommand>
{
    public UpdateCommentCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Body).NotEmpty();
        RuleFor(x => x.BlogId).NotEmpty();
    }
}
