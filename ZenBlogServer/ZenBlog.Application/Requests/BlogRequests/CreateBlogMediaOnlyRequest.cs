using Microsoft.AspNetCore.Http;

namespace ZenBlog.Application.Requests.BlogRequests;

public sealed record CreateBlogMediaOnlyRequest(
    IFormFile CoverImage,
    IFormFile BlogImage);
