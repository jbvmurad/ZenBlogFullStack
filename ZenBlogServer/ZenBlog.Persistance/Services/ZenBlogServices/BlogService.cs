using AutoMapper;
using AutoMapper.QueryableExtensions;
using GenericRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ZenBlog.Application.Features.ZenBlogFeatures.BlogFeatures.Commands.CreateBlog;
using ZenBlog.Application.Features.ZenBlogFeatures.BlogFeatures.Commands.DeleteBlog;
using ZenBlog.Application.Features.ZenBlogFeatures.BlogFeatures.Commands.UpdateBlog;
using ZenBlog.Application.Services.Storage;
using ZenBlog.Application.Services.ZenBlogService;
using ZenBlog.Domain.DTOs.ZenBlogResponses;
using ZenBlog.Domain.Entities.ZenBlogEntities;
using ZenBlog.Domain.Repositories.ZenBlogRepositories;

namespace ZenBlog.Persistance.Services.ZenBlogServices;

public sealed class BlogService : IBlogService
{
    private readonly IMapper _mapper;
    private readonly IBlogRepository _blogRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorage _fileStorage;

    public BlogService(
        IMapper mapper,
        IBlogRepository blogRepository,
        IUnitOfWork unitOfWork,
        IFileStorage fileStorage)
    {
        _mapper = mapper;
        _blogRepository = blogRepository;
        _unitOfWork = unitOfWork;
        _fileStorage = fileStorage;
    }

    public IQueryable<BlogResponse> GetAllBlogs() => _blogRepository
        .GetAll()
        .AsNoTracking()
        .ProjectTo<BlogResponse>(_mapper.ConfigurationProvider);

    public async Task<string> CreateAsync(CreateBlogCommand request, CancellationToken cancellationToken)
    {
        string? coverUrl = null;
        string? blogImageUrl = null;

        try
        {
            coverUrl = await _fileStorage.SaveImageAsync(request.CoverImage, cancellationToken);
            blogImageUrl = await _fileStorage.SaveImageAsync(request.BlogImage, cancellationToken);

            var blog = _mapper.Map<Blog>(request);
            blog.CoverImage = coverUrl;
            blog.BlogImage = blogImageUrl;

            await _blogRepository.AddAsync(blog, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return blog.Id;
        }
        catch
        {
            if (!string.IsNullOrWhiteSpace(coverUrl))
                await _fileStorage.TryDeleteAsync(coverUrl, cancellationToken);

            if (!string.IsNullOrWhiteSpace(blogImageUrl) &&
                !string.Equals(blogImageUrl, coverUrl, StringComparison.OrdinalIgnoreCase))
            {
                await _fileStorage.TryDeleteAsync(blogImageUrl, cancellationToken);
            }

            throw;
        }
    }

    public async Task DeleteAsync(DeleteBlogCommand request, CancellationToken cancellationToken)
    {
        var blog = await _blogRepository.Where(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
        if (blog is null)
            throw new ArgumentException($"Blog with Id {request.Id} isn't found ");

        var oldCover = blog.CoverImage;
        var oldBlogImage = blog.BlogImage;

        _blogRepository.Delete(blog);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(oldCover))
            await _fileStorage.TryDeleteAsync(oldCover, cancellationToken);

        if (!string.IsNullOrWhiteSpace(oldBlogImage) &&
            !string.Equals(oldBlogImage, oldCover, StringComparison.OrdinalIgnoreCase))
        {
            await _fileStorage.TryDeleteAsync(oldBlogImage, cancellationToken);
        }
    }

    public async Task UpdateAsync(UpdateBlogCommand request, CancellationToken cancellationToken)
    {
        var blog = await _blogRepository.Where(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
        if (blog is null)
            throw new ArgumentException($"Blog with Id {request.Id} isn't found ");

        var oldCover = blog.CoverImage;
        var oldBlogImage = blog.BlogImage;
        string? newCoverUrl = null;
        string? newBlogImageUrl = null;

        try
        {
            if (!string.IsNullOrWhiteSpace(request.Title))
                blog.Title = request.Title;

            if (!string.IsNullOrWhiteSpace(request.Description))
                blog.Description = request.Description;

            if (!string.IsNullOrWhiteSpace(request.CategoryId))
                blog.CategoryId = request.CategoryId;

            if (!string.IsNullOrWhiteSpace(request.UserId))
                blog.UserId = request.UserId;

            if (request.CoverImage is not null)
            {
                newCoverUrl = await _fileStorage.SaveImageAsync(request.CoverImage, cancellationToken);
                blog.CoverImage = newCoverUrl;
            }

            if (request.BlogImage is not null)
            {
                newBlogImageUrl = await _fileStorage.SaveImageAsync(request.BlogImage, cancellationToken);
                blog.BlogImage = newBlogImageUrl;
            }

            _blogRepository.Update(blog);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            if (!string.IsNullOrWhiteSpace(newCoverUrl) &&
                !string.Equals(newCoverUrl, oldCover, StringComparison.OrdinalIgnoreCase))
            {
                await _fileStorage.TryDeleteAsync(newCoverUrl, cancellationToken);
            }

            if (!string.IsNullOrWhiteSpace(newBlogImageUrl) &&
                !string.Equals(newBlogImageUrl, oldBlogImage, StringComparison.OrdinalIgnoreCase))
            {
                await _fileStorage.TryDeleteAsync(newBlogImageUrl, cancellationToken);
            }

            throw;
        }

        if (!string.IsNullOrWhiteSpace(oldCover) &&
            !string.IsNullOrWhiteSpace(blog.CoverImage) &&
            !string.Equals(oldCover, blog.CoverImage, StringComparison.OrdinalIgnoreCase))
        {
            await _fileStorage.TryDeleteAsync(oldCover, cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(oldBlogImage) &&
            !string.IsNullOrWhiteSpace(blog.BlogImage) &&
            !string.Equals(oldBlogImage, blog.BlogImage, StringComparison.OrdinalIgnoreCase))
        {
            await _fileStorage.TryDeleteAsync(oldBlogImage, cancellationToken);
        }
    }

    public async Task<string> SaveBlogImageAsync(IFormFile media, CancellationToken cancellationToken)
    {
        return await _fileStorage.SaveImageAsync(media, cancellationToken);
    }
}
