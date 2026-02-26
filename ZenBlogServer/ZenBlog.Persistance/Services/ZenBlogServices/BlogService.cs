using AutoMapper;
using GenericRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ZenBlog.Application.Features.ZenBlogFeatures.BlogFeatures.Commands.CreateBlog;
using ZenBlog.Application.Features.ZenBlogFeatures.BlogFeatures.Commands.DeleteBlog;
using ZenBlog.Application.Features.ZenBlogFeatures.BlogFeatures.Commands.UpdateBlog;
using ZenBlog.Application.Services.Storage;
using ZenBlog.Application.Services.ZenBlogService;
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

    public IQueryable<Blog> GetAllBlogs() => _blogRepository.GetAll().AsQueryable();

    public async Task<Blog?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        return await _blogRepository.Where(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<string> CreateAsync(CreateBlogCommand request, CancellationToken cancellationToken)
    {
        Blog blog = _mapper.Map<Blog>(request);
        await _blogRepository.AddAsync(blog);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return blog.Id;
    }

    public async Task DeleteAsync(DeleteBlogCommand request, CancellationToken cancellationToken)
    {
        var blog = await _blogRepository.Where(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
        if (blog is null)
            throw new ArgumentException($"Blog with Id {request.Id} isn't found ");

        _blogRepository.Delete(blog);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

    }

    public async Task UpdateAsync(UpdateBlogCommand request, CancellationToken cancellationToken)
    {
        var blog = await _blogRepository.Where(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
        if (blog is null)
            throw new ArgumentException($"Blog with Id {request.Id} isn't found ");

        var oldCover = blog.CoverImage;
        var oldBlogImage = blog.BlogImage;

        _mapper.Map(request, blog);

        if (string.IsNullOrWhiteSpace(request.CoverImage))
            blog.CoverImage = oldCover;
        if (string.IsNullOrWhiteSpace(request.BlogImage))
            blog.BlogImage = oldBlogImage;

        _blogRepository.Update(blog);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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
