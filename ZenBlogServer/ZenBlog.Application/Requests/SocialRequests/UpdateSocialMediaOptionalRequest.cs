using Microsoft.AspNetCore.Http;

namespace ZenBlog.Application.Requests.SocialRequests;

public sealed record UpdateSocialMediaOptionalRequest(
    IFormFile? Icon);
