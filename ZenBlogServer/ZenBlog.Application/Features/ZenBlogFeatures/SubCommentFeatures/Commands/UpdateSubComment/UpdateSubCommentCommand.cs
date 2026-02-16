namespace ZenBlog.Application.Features.ZenBlogFeatures.SubCommentFeatures.Commands.UpdateSubComment;

public sealed record UpdateSubCommentCommand(
    string Id,
    string FirstName,
    string LastName,
    string Email,
    string Body,
    string CommentId);
