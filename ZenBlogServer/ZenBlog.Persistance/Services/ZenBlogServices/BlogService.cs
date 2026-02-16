using AutoMapper;
using GenericRepository;
using Microsoft.EntityFrameworkCore;
using ZenBlog.Application.Features.ZenBlogFeatures.BlogFeatures.Commands.CreateBlog;
using ZenBlog.Application.Features.ZenBlogFeatures.BlogFeatures.Commands.DeleteBlog;
using ZenBlog.Application.Features.ZenBlogFeatures.BlogFeatures.Commands.UpdateBlog;
using ZenBlog.Application.Services.ZenBlogService;
using ZenBlog.Domain.Entities.ZenBlogEntities;
using ZenBlog.Domain.Repositories.ZenBlogRepositories;

namespace ZenBlog.Persistance.Services.ZenBlogServices;

public sealed class BlogService : IBlogService
{
    private readonly IMapper _mapper;
    private readonly IBlogRepository _blogRepository;
    private readonly IUnitOfWork _unitOfWork;
    public BlogService(IMapper mapper, IBlogRepository blogRepository, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _blogRepository = blogRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task CreateAsync(CreateBlogCommand request, CancellationToken cancellationToken)
    {
        Blog blog = _mapper.Map<Blog>(request);
        await _blogRepository.AddAsync(blog);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(DeleteBlogCommand request, CancellationToken cancellationToken)
    {
        var blog = await _blogRepository.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
        if(blog is null) throw new ArgumentException($"Blog with Id {request.Id} isn't found ");
        _blogRepository.Delete(blog);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public IQueryable<Blog> GetAllBlogs() =>_blogRepository.GetAll().AsQueryable();

    public async Task UpdateAsync(UpdateBlogCommand request, CancellationToken cancellationToken)
    {
        var blog = await _blogRepository.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
        if (blog is null) throw new ArgumentException($"Blog with Id {request.Id} isn't found ");
        _blogRepository.Update(blog);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
