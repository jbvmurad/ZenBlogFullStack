using AutoMapper;
using GenericRepository;
using Microsoft.EntityFrameworkCore;
using ZenBlog.Application.Features.ZenBlogFeatures.ContactInfoFeatures.Commands.CreateContactInfo;
using ZenBlog.Application.Features.ZenBlogFeatures.ContactInfoFeatures.Commands.DeleteContactInfo;
using ZenBlog.Application.Features.ZenBlogFeatures.ContactInfoFeatures.Commands.UpdateContactInfo;
using ZenBlog.Application.Services.ZenBlogService;
using ZenBlog.Domain.Entities.ZenBlogEntities;
using ZenBlog.Domain.Repositories.ZenBlogRepositories;

namespace ZenBlog.Persistance.Services.ZenBlogServices;

public sealed class ContactInfoService : IContactInfoService
{
    private readonly IContactInfoRepository _contactInfoRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public ContactInfoService(IContactInfoRepository contactInfoRepository, IMapper mapper, IUnitOfWork unitOfWork)
    {
        _contactInfoRepository = contactInfoRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task CreateAsync(CreateContactInfoCommand request, CancellationToken cancellationToken)
    {
        var exists = await _contactInfoRepository
            .GetAll()
            .AsNoTracking()
            .AnyAsync(cancellationToken);

        if (exists)
            throw new InvalidOperationException("ContactInfo already exists. Use Update instead.");

        var entity = _mapper.Map<ContactInfo>(request);

        await _contactInfoRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(DeleteContactInfoCommand request, CancellationToken cancellationToken)
    {
        var ContactInfo = await _contactInfoRepository
            .GetAll()
            .FirstOrDefaultAsync(cancellationToken);

        if (ContactInfo is null)
            return;

        _contactInfoRepository.Delete(ContactInfo);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public IQueryable<ContactInfo> GetContactInfo()
    {
        return _contactInfoRepository.GetAll().AsQueryable();
    }

    public async Task UpdateAsync(UpdateContactInfoCommand request, CancellationToken cancellationToken)
    {
        var ContactInfo = await _contactInfoRepository
            .GetAll()
            .FirstOrDefaultAsync(cancellationToken);

        if (ContactInfo is null)
            throw new KeyNotFoundException("ContactInfo not found. Create it first.");

        _mapper.Map(request, ContactInfo);

        _contactInfoRepository.Update(ContactInfo);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
