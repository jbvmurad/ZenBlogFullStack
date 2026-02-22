using Microsoft.AspNetCore.Http;

namespace ZenBlog.Application.Services.Storage;

public interface IFileStorage
{
    Task<string> SaveImageAsync(IFormFile media, CancellationToken cancellationToken);

    Task TryDeleteAsync(string? mediaUrl, CancellationToken cancellationToken);
}
