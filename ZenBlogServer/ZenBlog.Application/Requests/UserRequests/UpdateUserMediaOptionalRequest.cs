using Microsoft.AspNetCore.Http;

namespace ZenBlog.Application.Requests.UserRequests;

public sealed record UpdateUserMediaOptionalRequest(
    IFormFile? Image);
