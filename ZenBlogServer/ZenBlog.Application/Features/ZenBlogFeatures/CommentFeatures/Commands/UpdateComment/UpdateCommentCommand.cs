namespace ZenBlog.Application.Features.ZenBlogFeatures.CommentFeatures.Commands.UpdateComment;

public sealed record UpdateCommentCommand(
    string Id,
    string FirstName,
    string LastName,
    string Email,
    string Body,
    string BlogId);
