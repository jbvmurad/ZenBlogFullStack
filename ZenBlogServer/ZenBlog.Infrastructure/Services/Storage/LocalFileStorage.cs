using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ZenBlog.Application.Services.Storage;

namespace ZenBlog.Infrastructure.Services.Storage;

public sealed class LocalFileStorage : IFileStorage
{
    private readonly IWebHostEnvironment _env;

    public LocalFileStorage(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<string> SaveImageAsync(IFormFile media, CancellationToken cancellationToken)
    {
        ValidateImage(media);

        var ext = Path.GetExtension(media.FileName).ToLowerInvariant();

        var uploadsRoot = Path.Combine(_env.ContentRootPath, "uploads");
        Directory.CreateDirectory(uploadsRoot);

        var fileName = $"{Guid.NewGuid():N}{ext}";
        var filePath = Path.Combine(uploadsRoot, fileName);

        await using (var stream = File.Create(filePath))
        {
            await media.CopyToAsync(stream, cancellationToken);
        }

        return $"/uploads/{fileName}";
    }

    public Task TryDeleteAsync(string? mediaUrl, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(mediaUrl))
            return Task.CompletedTask;

        var normalized = mediaUrl.Replace('\\', '/');
        if (normalized.StartsWith("/"))
            normalized = normalized[1..];

        if (!normalized.StartsWith("uploads/", StringComparison.OrdinalIgnoreCase))
            return Task.CompletedTask;

        var fileName = Path.GetFileName(normalized);
        if (string.IsNullOrWhiteSpace(fileName) || fileName.Contains("..", StringComparison.Ordinal))
            return Task.CompletedTask;

        var uploadsRoot = Path.Combine(_env.ContentRootPath, "uploads");
        var filePath = Path.Combine(uploadsRoot, fileName);

        try
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
        catch
        {
        }

        return Task.CompletedTask;
    }

    private static void ValidateImage(IFormFile media)
    {
        if (media is null || media.Length == 0)
            throw ToValidationException("Image file is required");

        if (!media.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            throw ToValidationException("Only image files are allowed");

        var ext = Path.GetExtension(media.FileName).ToLowerInvariant();
        var allowedExt = new HashSet<string> { ".jpg", ".jpeg", ".png", ".webp" };
        if (!allowedExt.Contains(ext))
            throw ToValidationException("Unsupported image format. Use jpg/jpeg/png/webp");

        const long maxBytes = 10 * 1024 * 1024;
        if (media.Length > maxBytes)
            throw ToValidationException("Image is too large. Max 10MB");
    }

    private static ValidationException ToValidationException(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            message = "Validation failed";

        return new ValidationException(new[]
        {
            new ValidationFailure("", message)
        });
    }
}
