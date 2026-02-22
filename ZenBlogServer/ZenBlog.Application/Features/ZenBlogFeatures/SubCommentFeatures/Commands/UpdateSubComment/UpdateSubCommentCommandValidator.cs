using FluentValidation;

namespace ZenBlog.Application.Features.ZenBlogFeatures.SubCommentFeatures.Commands.UpdateSubComment;

public sealed class UpdateSubCommentCommandValidator : AbstractValidator<UpdateSubCommentCommand>
{
    public UpdateSubCommentCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Body).NotEmpty();
        RuleFor(x => x.CommentId).NotEmpty();
    }
}
