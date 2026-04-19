using AutoMapper;
using AutoMapper.QueryableExtensions;
using GenericRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ZenBlog.Application.Features.ZenBlogFeatures.SocialFeatures.Commands.CreateSocial;
using ZenBlog.Application.Features.ZenBlogFeatures.SocialFeatures.Commands.DeleteSocial;
using ZenBlog.Application.Features.ZenBlogFeatures.SocialFeatures.Commands.UpdateSocial;
using ZenBlog.Application.Services.Storage;
using ZenBlog.Application.Services.ZenBlogService;
using ZenBlog.Domain.DTOs.ZenBlogResponses;
using ZenBlog.Domain.Entities.ZenBlogEntities;
using ZenBlog.Domain.Repositories.ZenBlogRepositories;

namespace ZenBlog.Persistance.Services.ZenBlogServices;

public sealed class SocialService : ISocialService
{
    private readonly ISocialRepository _socialRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IFileStorage _fileStorage;

    public SocialService(ISocialRepository socialRepository, IUnitOfWork unitOfWork, IMapper mapper, IFileStorage fileStorage)
    {
        _socialRepository = socialRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _fileStorage = fileStorage;
    }

    public async Task CreateAsync(CreateSocialCommand request, CancellationToken cancellationToken)
    {
        var count = await _socialRepository.GetAll().CountAsync(cancellationToken);

        if (count >= 4)
            throw new InvalidOperationException("You can create up to 4 social records only.");

        string? iconUrl = null;

        try
        {
            iconUrl = await _fileStorage.SaveImageAsync(request.Icon, cancellationToken);

            var entity = _mapper.Map<Social>(request);
            entity.Icon = iconUrl;

            await _socialRepository.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            if (!string.IsNullOrWhiteSpace(iconUrl))
                await _fileStorage.TryDeleteAsync(iconUrl, cancellationToken);

            throw;
        }
    }

    public async Task DeleteAsync(DeleteSocialCommand request, CancellationToken cancellationToken)
    {
        var social = await _socialRepository
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (social is null)
            throw new KeyNotFoundException("Social record not found.");

        var oldIcon = social.Icon;

        _socialRepository.Delete(social);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(oldIcon))
            await _fileStorage.TryDeleteAsync(oldIcon, cancellationToken);
    }

    public IQueryable<Social> GetAllSocial() => _socialRepository.GetAll().AsNoTracking();

    public async Task<SocialResponse?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        return await _socialRepository
            .Where(x => x.Id == id)
            .AsNoTracking()
            .ProjectTo<SocialResponse>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<string> SaveSocialIconAsync(IFormFile media, CancellationToken cancellationToken)
    {
        return await _fileStorage.SaveImageAsync(media, cancellationToken);
    }
    public async Task UpdateAsync(UpdateSocialCommand request, CancellationToken cancellationToken)
    {
        var social = await _socialRepository
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (social is null)
            throw new KeyNotFoundException("Social record not found.");

        var oldIcon = social.Icon;
        string? newIconUrl = null;

        try
        {
            if (!string.IsNullOrWhiteSpace(request.Title))
                social.Title = request.Title;

            if (!string.IsNullOrWhiteSpace(request.Url))
                social.Url = request.Url;

            if (request.Icon is not null)
            {
                newIconUrl = await _fileStorage.SaveImageAsync(request.Icon, cancellationToken);
                social.Icon = newIconUrl;
            }

            _socialRepository.Update(social);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            if (!string.IsNullOrWhiteSpace(newIconUrl) &&
                !string.Equals(newIconUrl, oldIcon, StringComparison.OrdinalIgnoreCase))
            {
                await _fileStorage.TryDeleteAsync(newIconUrl, cancellationToken);
            }

            throw;
        }

        if (!string.IsNullOrWhiteSpace(oldIcon) &&
            !string.IsNullOrWhiteSpace(social.Icon) &&
            !string.Equals(oldIcon, social.Icon, StringComparison.OrdinalIgnoreCase))
        {
            await _fileStorage.TryDeleteAsync(oldIcon, cancellationToken);
        }
    }
}
