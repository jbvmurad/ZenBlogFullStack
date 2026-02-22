using Amazon.S3;
using Amazon.S3.Model;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using ZenBlog.Application.Options;
using ZenBlog.Application.Services.Storage;

namespace ZenBlog.Infrastructure.Services.Storage;

public sealed class S3FileStorage : IFileStorage
{
    private readonly IAmazonS3 _s3;
    private readonly AwsS3Options _options;
    private readonly SemaphoreSlim _bucketLock = new(1, 1);
    private bool _bucketEnsured;

    public S3FileStorage(IAmazonS3 s3, IOptions<AwsS3Options> options)
    {
        _s3 = s3;
        _options = options.Value;
    }

    public async Task<string> SaveImageAsync(IFormFile media, CancellationToken cancellationToken)
    {
        ValidateImage(media);

        await EnsureBucketExistsAsync(cancellationToken);

        var ext = Path.GetExtension(media.FileName).ToLowerInvariant();
        var keyPrefix = string.IsNullOrWhiteSpace(_options.KeyPrefix) ? "uploads/" : _options.KeyPrefix;
        if (!keyPrefix.EndsWith("/"))
            keyPrefix += "/";

        var key = $"{keyPrefix}{Guid.NewGuid():N}{ext}";

        var put = new PutObjectRequest
        {
            BucketName = _options.BucketName,
            Key = key,
            InputStream = media.OpenReadStream(),
            ContentType = media.ContentType,
        };

        if (_options.MakePublic)
        {
            put.CannedACL = S3CannedACL.PublicRead;
        }

        await _s3.PutObjectAsync(put, cancellationToken);

        var baseUrl = _options.PublicBaseUrl;
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            baseUrl = $"https://{_options.BucketName}.s3.{_options.Region}.amazonaws.com";
        }

        return $"{baseUrl.TrimEnd('/')}/{key}";
    }

    private async Task EnsureBucketExistsAsync(CancellationToken cancellationToken)
    {
        if (_bucketEnsured || !_options.Enabled)
            return;

        await _bucketLock.WaitAsync(cancellationToken);
        try
        {
            if (_bucketEnsured)
                return;

            try
            {
                await _s3.PutBucketAsync(new PutBucketRequest { BucketName = _options.BucketName }, cancellationToken);
            }
            catch (AmazonS3Exception ex) when (
                ex.StatusCode == System.Net.HttpStatusCode.Conflict ||
                ex.ErrorCode == "BucketAlreadyOwnedByYou" ||
                ex.ErrorCode == "BucketAlreadyExists")
            {
            }

            _bucketEnsured = true;
        }
        finally
        {
            _bucketLock.Release();
        }
    }

    public async Task TryDeleteAsync(string? mediaUrl, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(mediaUrl))
            return;

        try
        {
            var uri = new Uri(mediaUrl, UriKind.Absolute);
            var key = uri.AbsolutePath;
            if (key.StartsWith("/"))
                key = key[1..];

            if (string.IsNullOrWhiteSpace(key))
                return;

            await _s3.DeleteObjectAsync(new DeleteObjectRequest
            {
                BucketName = _options.BucketName,
                Key = key
            }, cancellationToken);
        }
        catch
        {
        }
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
