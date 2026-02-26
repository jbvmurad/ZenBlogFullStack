using Microsoft.AspNetCore.Http;

namespace ZenBlog.Application.Requests.SocialRequests;

public sealed record CreateSocialMediaOnlyRequest(
    IFormFile Icon);
