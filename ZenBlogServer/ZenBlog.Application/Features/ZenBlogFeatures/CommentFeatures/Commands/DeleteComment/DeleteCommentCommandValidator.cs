using FluentValidation;

namespace ZenBlog.Application.Features.ZenBlogFeatures.CommentFeatures.Commands.DeleteComment;

public sealed class DeleteCommentCommandValidator : AbstractValidator<DeleteCommentCommand>
{
    public DeleteCommentCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
