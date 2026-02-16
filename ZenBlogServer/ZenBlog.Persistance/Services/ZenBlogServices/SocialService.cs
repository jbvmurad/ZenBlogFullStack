using AutoMapper;
using GenericRepository;
using Microsoft.EntityFrameworkCore;
using ZenBlog.Application.Features.ZenBlogFeatures.SocialFeatures.Commands.CreateSocial;
using ZenBlog.Application.Features.ZenBlogFeatures.SocialFeatures.Commands.DeleteSocial;
using ZenBlog.Application.Features.ZenBlogFeatures.SocialFeatures.Commands.UpdateSocial;
using ZenBlog.Application.Services.ZenBlogService;
using ZenBlog.Domain.Entities.ZenBlogEntities;
using ZenBlog.Domain.Repositories.ZenBlogRepositories;

namespace ZenBlog.Persistance.Services.ZenBlogServices;

public sealed class SocialService : ISocialService
{
    private readonly ISocialRepository _socialRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SocialService(ISocialRepository socialRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _socialRepository = socialRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task CreateAsync(CreateSocialCommand request, CancellationToken cancellationToken)
    {
        var count = await _socialRepository.GetAll().CountAsync(cancellationToken);

        if (count >= 4)
            throw new InvalidOperationException("You can create up to 4 social records only.");

        var entity = _mapper.Map<Social>(request);

        await _socialRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(DeleteSocialCommand request, CancellationToken cancellationToken)
    {
        var social = await _socialRepository
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (social is null)
            throw new KeyNotFoundException("Social record not found.");

        _socialRepository.Delete(social);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public IQueryable<Social> GetAllSocial()
    {
        return _socialRepository.GetAll().AsQueryable();
    }

    public async Task UpdateAsync(UpdateSocialCommand request, CancellationToken cancellationToken)
    {
        var social = await _socialRepository
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (social is null)
            throw new KeyNotFoundException("Social record not found.");

        _mapper.Map(request, social);

        _socialRepository.Update(social);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
