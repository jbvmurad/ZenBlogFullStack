namespace ZenBlog.Application.Features.ZenBlogFeatures.SubCommentFeatures.Commands.CreateSubComment;

public sealed record CreateSubCommentCommand(
    string FirstName,
    string LastName,
    string Email,
    string Body,
    string CommentId);
