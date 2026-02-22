using Microsoft.AspNetCore.Http;

namespace ZenBlog.Application.Requests.BlogRequests;

public sealed record UpdateBlogMediaOptionalRequest(
    IFormFile? CoverImage,
    IFormFile? BlogImage);
