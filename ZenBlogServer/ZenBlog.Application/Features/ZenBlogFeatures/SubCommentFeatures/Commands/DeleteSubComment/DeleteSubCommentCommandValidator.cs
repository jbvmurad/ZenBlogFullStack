using FluentValidation;

namespace ZenBlog.Application.Features.ZenBlogFeatures.SubCommentFeatures.Commands.DeleteSubComment;

public sealed class DeleteSubCommentCommandValidator : AbstractValidator<DeleteSubCommentCommand>
{
    public DeleteSubCommentCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
