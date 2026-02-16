namespace ZenBlog.Application.Features.ZenBlogFeatures.CommentFeatures.Commands.CreateComment;

public sealed record CreateCommentCommand(
    string FirstName,
    string LastName,
    string Email,
    string Body,
    string BlogId);
