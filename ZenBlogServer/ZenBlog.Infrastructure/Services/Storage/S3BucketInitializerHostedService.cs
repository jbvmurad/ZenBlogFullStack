using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ZenBlog.Application.Options;

namespace ZenBlog.Infrastructure.Services.Storage;

public sealed class S3BucketInitializerHostedService : IHostedService
{
    private readonly IAmazonS3 _s3;
    private readonly AwsS3Options _options;
    private readonly ILogger<S3BucketInitializerHostedService> _logger;

    public S3BucketInitializerHostedService(
        IAmazonS3 s3,
        IOptions<AwsS3Options> options,
        ILogger<S3BucketInitializerHostedService> logger)
    {
        _s3 = s3;
        _options = options.Value;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_options.Enabled || string.IsNullOrWhiteSpace(_options.BucketName))
            return;

        try
        {
            await _s3.PutBucketAsync(new PutBucketRequest
            {
                BucketName = _options.BucketName
            }, cancellationToken);

            _logger.LogInformation("S3 bucket '{BucketName}' created.", _options.BucketName);
        }
        catch (AmazonS3Exception ex) when (
            ex.StatusCode == HttpStatusCode.Conflict ||
            ex.ErrorCode == "BucketAlreadyOwnedByYou")
        {
            _logger.LogInformation("S3 bucket '{BucketName}' already exists (owned by you).", _options.BucketName);
        }
        catch (AmazonS3Exception ex) when (ex.ErrorCode == "BucketAlreadyExists")
        {
            _logger.LogWarning(ex, "S3 bucket '{BucketName}' already exists but might not be owned by this account.", _options.BucketName);
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.Forbidden)
        {
            _logger.LogWarning(ex, "S3 bucket '{BucketName}' exists but access is forbidden. Skipping creation.", _options.BucketName);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Unable to verify/create S3 bucket '{BucketName}' at startup.", _options.BucketName);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
