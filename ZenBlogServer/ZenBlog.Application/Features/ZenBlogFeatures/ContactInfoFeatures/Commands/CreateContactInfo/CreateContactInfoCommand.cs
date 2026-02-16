namespace ZenBlog.Application.Features.ZenBlogFeatures.ContactInfoFeatures.Commands.CreateContactInfo;

public sealed record CreateContactInfoCommand(
    string Address,
    string Email,
    string Phone,
    string MapUrl);
