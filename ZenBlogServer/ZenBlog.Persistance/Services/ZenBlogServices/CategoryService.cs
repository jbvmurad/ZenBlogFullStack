using AutoMapper;
using GenericRepository;
using Microsoft.EntityFrameworkCore;
using ZenBlog.Application.Features.ZenBlogFeatures.CategoryFeatures.Commands.CreateCategory;
using ZenBlog.Application.Features.ZenBlogFeatures.CategoryFeatures.Commands.DeleteCategory;
using ZenBlog.Application.Features.ZenBlogFeatures.CategoryFeatures.Commands.UpdateCategory;
using ZenBlog.Application.Services.ZenBlogService;
using ZenBlog.Domain.Entities.ZenBlogEntities;
using ZenBlog.Domain.Repositories.ZenBlogRepositories;

namespace ZenBlog.Persistance.Services.ZenBlogServices;

public sealed class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(ICategoryRepository categoryRepository, IMapper mapper, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task CreateAsync(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        Category category=_mapper.Map<Category>(request);
        await _categoryRepository.AddAsync(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category=await _categoryRepository.Where(x=>x.Id==request.Id).FirstOrDefaultAsync();
        if (category == null) throw new ArgumentException($"Category with Id {request.Id} isn't found");
        _categoryRepository.Delete(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public IQueryable<Category> GetAllCategories() => _categoryRepository.GetAll().AsQueryable();

    public async Task UpdateAsync(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
        if (category == null) throw new ArgumentException($"Category with Id {request.Id} isn't found");

        _mapper.Map(request,category);
        _categoryRepository.Update(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
