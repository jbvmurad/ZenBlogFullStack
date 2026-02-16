namespace ZenBlog.Application.Features.ZenBlogFeatures.ContactInfoFeatures.Commands.UpdateContactInfo;

public sealed record UpdateContactInfoCommand(
    string Id,
    string Address,
    string Email,
    string Phone,
    string MapUrl);
